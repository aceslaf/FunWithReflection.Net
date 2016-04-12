using ClinicHQ.Data.Auditing.Models;
using System.Collections.Generic;

namespace ClinicHQ.Data.Auditing.Interfaces
{
    public interface ILogDetailsAuditor : IAuditor
    {
        List<AuditLogDetail> GetLogDetails();
    }

    public interface IAuditor
    {
        void BeforeSaveChanges();
        void AfterSaveChanges();
    }
}