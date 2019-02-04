using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("audit_logs")]
    public class EntityAuditLog
    {
        public EntityAuditLog()
        {
            DateTime = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("audit_log_id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("audit_type")]
        public EnumAuditEntry.AuditType AuditType { get; set; }

        [Column("object_type")]
        public string ObjectType { get; set; }

        [Column("object_id")]
        public int ObjectId { get; set; }

        [Column("object_name")]
        public string ObjectName { get; set; }

        [Column("date_time_local")]
        public DateTime DateTime { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("object_json")]
        public string ObjectJson { get; set; }


       
       

       
    }
}