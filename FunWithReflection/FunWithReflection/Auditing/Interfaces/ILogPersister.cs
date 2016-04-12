using ClinicHQ.Data.Auditing.Models;
using System.Collections.Generic;

namespace ClinicHQ.Data.Auditing
{
    public interface ILogPersister
    {
        void SaveLogs(IEnumerable<AuditLog> logs);
    }
}
