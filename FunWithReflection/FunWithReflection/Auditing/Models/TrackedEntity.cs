using System;

namespace ClinicHQ.Data.Auditing.Models
{
    public class TrackedEntity<T>
    {
        public int UserId { get; set; }
        public int ClinicId { get; set; }
        public DateTime TimeStamp { get; set; }
        public T Entity { get; set; }

        public TrackedEntity(T entity, AuditLog log)
        {
            UserId = log.UserId;
            ClinicId = log.ClinicId;
            TimeStamp = log.EventDateUTC;
            Entity = entity;
        }
    }
}
