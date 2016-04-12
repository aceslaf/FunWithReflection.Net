using ClinicHQ.Data.Auditing.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicHQ.Data.Auditing.Models
{
    /// <summary>
    ///     This model class is used to store the changes made in datbase values
    ///     For the audit purpose. Only selected tables can be tracked with the help of TrackChangesAttribute Attribute present
    ///     in the common library.
    /// </summary>
    public class AuditLog : IUnTrackable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AuditLogId { get; set; }

        public int UserId { get; set; }
        public int ClinicId { get; set; }

        [Required]
        public DateTime EventDateUTC { get; set; }

        [Required]
        public EventType EventType { get; set; }

        [Required]
        public int TypeId { get; set; }

        public int RecordId { get; set; }

        public virtual ICollection<AuditLogDetail> LogDetails { get; set; } = new List<AuditLogDetail>();
    }
}