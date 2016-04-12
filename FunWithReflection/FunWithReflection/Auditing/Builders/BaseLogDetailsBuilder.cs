using ClinicHQ.Data.Auditing.Configuration;
using ClinicHQ.Data.Auditing.Exstensions;
using ClinicHQ.Data.Auditing.Interfaces;
using ClinicHQ.Data.Auditing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;


namespace ClinicHQ.Data.Auditing.Builders
{
    public abstract class BaseLogDetailsBuilder : ILogDetailsAuditor
    {
        protected readonly DbEntityEntry DbEntry;
        private List<AuditLogDetail> _logDetails { get; set; }
        public BaseLogDetailsBuilder(DbEntityEntry dbEntry)
        {
            DbEntry = dbEntry;
            _logDetails = new List<AuditLogDetail>();
        }

        #region PublicApi
        public List<AuditLogDetail> GetLogDetails()
        {
            return _logDetails;
        }

        public abstract void BeforeSaveChanges();

        public abstract void AfterSaveChanges();

        #endregion

        #region Core
        protected void CreateLogDetails()
        {
            Type entityType = DbEntry.Entity.GetType().GetEntityType();

            foreach (string propertyName in PropertyNamesOfEntity())
            {
                if (EntityTrackingConfiguration.IsPropertyTracked(entityType, propertyName)
                    && IsValueChanged(propertyName))
                {
                    _logDetails.Add(new AuditLogDetail
                    {
                        PropertyName = propertyName,
                        OriginalValue = OriginalValue(propertyName)?.ToString(),
                        NewValue = CurrentValue(propertyName)?.ToString()
                    });
                }
            }
        }
        #endregion

        #region OveridableApi
        protected abstract bool IsValueChanged(string propertyName);
        protected virtual IEnumerable<string> PropertyNamesOfEntity()
        {
            return DbEntry.OriginalValues.PropertyNames;
        }
        protected virtual object OriginalValue(string propertyName)
        {
            object originalValue = null;

            if (GlobalTrackingConfig.DisconnectedContext)
            {
                originalValue = DbEntry.GetDatabaseValues().GetValue<object>(propertyName);
            }
            else
            {
                originalValue = DbEntry.Property(propertyName).OriginalValue;
            }

            return originalValue;
        }

        protected virtual object CurrentValue(string propertyName)
        {
            var value = DbEntry.Property(propertyName).CurrentValue;
            return value;
        }
        #endregion
    }
}
