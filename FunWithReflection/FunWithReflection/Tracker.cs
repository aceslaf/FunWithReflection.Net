using FunWithReflection.Interfaces;
using ClinicHQ.Data.Auditing.Builders;
using ClinicHQ.Data.Auditing.Configuration;
using ClinicHQ.Data.Auditing.Exstensions;
using ClinicHQ.Data.Auditing.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace ClinicHQ.Data.Auditing
{
    public class Tracker : IAuditor
    {
        private ISystemUser _systemUser;
        private DbContext _dbContext;
        private ILogPersister _logPersister;
        private List<LogBuilder> _logBuilders;
        public Tracker(DbContext context, ISystemUser user, ILogPersister persister)
        {
            _dbContext = context;
            _systemUser = user;
            _logPersister = persister;
            _logBuilders = new List<LogBuilder>();
        }
        public void AfterSaveChanges()
        {
            _logBuilders.ForEach(x => x.AfterSaveChanges());
            var logs = _logBuilders.Select(x => x.CreateLogRecord(_systemUser));
            _logPersister.SaveLogs(logs.Where(x => x != null));

        }


        public void BeforeSaveChanges()
        {
            _logBuilders = _dbContext.ChangeTracker
                           .Entries()
                           .Where(p =>
                                       (p.State == EntityState.Deleted || p.State == EntityState.Modified || p.State == EntityState.Added)
                                        &&
                                        EntityTrackingConfiguration.IsTrackingEnabled(p.Entity.GetType().GetEntityType())
                                 )
                           .Select(x => new LogBuilder(x, _dbContext))
                           .ToList();
            _logBuilders.ForEach(x => x.BeforeSaveChanges());
        }
    }
}
