using ClinicHQ.Data.Auditing.Models;
using FunWithReflection.Auditing.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithReflection.Core
{
    public class StateRestore
    {
        public IList<TrackedEntity<T>> History<T>(List<AuditLog> logs) where T : new()
        {
            var entities = new List<TrackedEntity<T>>();
            if (logs.Count <= 0)
            {
                return entities;
            }

            logs = logs.OrderBy(x => x.EventDateUTC).ToList();
            T prev = default(T);
            foreach (var log in logs)
            {
                var entity = AggregateLog(prev, log);
                entities.Add(new TrackedEntity<T>(entity, log));
                prev = entity;
            }

            return entities;
        }

        private TEntity AggregateLog<TEntity>(TEntity old, AuditLog log) where TEntity : new()
        {
            var entityType = typeof(TEntity);

            TEntity res;
            if (old == null)
            {
                res = new TEntity();
            }
            else
            {
                res = FirstOrderClone(old);
            }

            foreach (var logDetail in log.LogDetails)
            {
                var property = entityType.GetProperty(logDetail.PropertyName);
                var value = logDetail.NewValue;
                Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                object safeValue = (value == null) ? null : Convert.ChangeType(value, propertyType);
                property.SetValue(res, safeValue);
            }

            return res;
        }

        private TEntity FirstOrderClone<TEntity>(TEntity old) where TEntity : new()
        {
            var clone = new TEntity();
            foreach (var property in typeof(TEntity).GetProperties())
            {
                if (!(property.CanRead && property.CanWrite))
                {
                    continue;
                }
                var val = property.GetValue(old);
                property.SetValue(clone, val);
            }
            return clone;
        }
    }
}
