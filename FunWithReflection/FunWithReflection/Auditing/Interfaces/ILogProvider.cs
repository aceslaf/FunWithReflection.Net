using ClinicHQ.Data.Auditing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithReflection.Auditing.Interfaces
{
    public interface ILogProvider
    {

        IQueryable<AuditLog> GetLogs<TEntity>();
        IQueryable<AuditLog> GetLogs(int entityTypeName);
        IQueryable<AuditLog> GetLogs<TEntity>(int primaryKey);
        IQueryable<AuditLog> GetLogs(int entityTypeName, int primaryKey);
        IList<TrackedEntity<T>> History<T>(int recordId) where T : new();
    }
}
