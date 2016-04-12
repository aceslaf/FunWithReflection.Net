﻿using ClinicHQ.Data.Auditing.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicHQ.Data.Auditing.Models
{
    public class AuditLogDetail : IUnTrackable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string PropertyName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public virtual long AuditLogId { get; set; }

        [ForeignKey("AuditLogId")]
        public virtual AuditLog Log { get; set; }
    }
}