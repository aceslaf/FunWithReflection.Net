using ClinicHQ.Data.Auditing.Configuration;
using ClinicHQ.Data.Auditing.Configuration.Mapping;
using ClinicHQ.Data.Auditing.Exstensions;
using ClinicHQ.Data.Auditing.Interfaces;
using ClinicHQ.Data.Auditing.Models;
using FunWithReflection.External;
using FunWithReflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace ClinicHQ.Data.Auditing.Builders
{
    public class LogBuilder : IDisposable, IAuditor
    {
        public DbEntityEntry DbEntry { get; private set; }
        private readonly EventType _eventType;
        private readonly BaseLogDetailsBuilder _changeLogDetailsAuditor;
        private readonly DbContext _context;
        private int RecordId;

        public LogBuilder(DbEntityEntry dbEntry, DbContext context)
        {
            DbEntry = dbEntry;
            _context = context;
            _eventType = CreateEventType(dbEntry);
            _changeLogDetailsAuditor = CreateLogDetailsAuditor(_eventType);
        }



        public void Dispose()
        {
        }
        public void BeforeSaveChanges()
        {
            if (_eventType != EventType.Added)
            {
                RecordId = CreateRecordId();
            }
            _changeLogDetailsAuditor.BeforeSaveChanges();
        }

        public void AfterSaveChanges()
        {
            if (_eventType == EventType.Added)
            {
                RecordId = CreateRecordId();
            }
            _changeLogDetailsAuditor.AfterSaveChanges();
        }
        public AuditLog CreateLogRecord(ISystemUser user)
        {
            Type entityType = DbEntry.Entity.GetType().GetEntityType();

            if (!EntityTrackingConfiguration.IsTrackingEnabled(entityType))
            {
                return null;
            }

            DateTime changeTime = DateTime.UtcNow;
            var newlog = new AuditLog
            {
                UserId = user.UserId,
                ClinicId = user.ClinicId,
                EventDateUTC = changeTime,
                EventType = _eventType,
                TypeId = EntityTrackingConfiguration.GetTypeId(entityType),
                RecordId = RecordId
            };

            var logDetails = _changeLogDetailsAuditor.GetLogDetails();
            logDetails.ForEach(x => x.Log = newlog);
            newlog.LogDetails = logDetails;

            if (newlog.LogDetails.Any())
                return newlog;
            else
                return null;
        }

        private BaseLogDetailsBuilder CreateLogDetailsAuditor(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Added:
                    return new AdditionLogDetailsBuilder(DbEntry);

                case EventType.Deleted:
                    return new DeletionLogDetailsBuilder(DbEntry);

                case EventType.Modified:
                case EventType.SoftDeleted:
                case EventType.UnDeleted:
                    return new ChangeLogDetailsBuilder(DbEntry);
                default:
                    throw new NotSupportedException(string.Format("Value {0} of enum {1} is not supported", eventType, typeof(EventType)));
            }
        }

        public int CreateRecordId()
        {
            //var mapping = new PrimaryKeyMap(_context, DbEntry.Entity.GetType().GetEntityType());
            //List<PropertyConfiguerationKey> keyNames = mapping.PrimaryKeys().ToList();
            //return GetPrimaryKeyValuesOf(DbEntry, keyNames).ToString();
            return (DbEntry.Entity as EntityBase<int>).Id;
        }

        private static object GetPrimaryKeyValuesOf(
            DbEntityEntry dbEntry,
            List<PropertyConfiguerationKey> properties)
        {
            if (properties.Count == 1)
            {
                return dbEntry.GetDatabaseValues().GetValue<object>(properties.Select(x => x.PropertyName).First());
            }
            if (properties.Count > 1)
            {
                string output = "[";

                output += string.Join(",",
                    properties.Select(colName => dbEntry.GetDatabaseValues().GetValue<object>(colName.PropertyName)));

                output += "]";
                return output;
            }
            throw new KeyNotFoundException("key not found for " + dbEntry.Entity.GetType().FullName);
        }

        private EventType CreateEventType(DbEntityEntry entry)
        {
            if (entry.State == EntityState.Added)
            {
                return EventType.Added;
            }

            var isSoftDeletable = GlobalTrackingConfig.SoftDeletableType?.IsInstanceOfType(entry.Entity);

            if (isSoftDeletable != null && isSoftDeletable.Value)
            {
                var previouslyDeleted = (bool)entry.OriginalValues[GlobalTrackingConfig.SoftDeletablePropertyName];
                var nowDeleted = (bool)entry.CurrentValues[GlobalTrackingConfig.SoftDeletablePropertyName];

                if (previouslyDeleted && !nowDeleted)
                {
                    return EventType.UnDeleted;
                }

                if (!previouslyDeleted && nowDeleted)
                {
                    return EventType.SoftDeleted;
                }
            }

            var eventType = entry.State == EntityState.Modified ? EventType.Modified : EventType.Deleted;
            return eventType;
        }


    }
}