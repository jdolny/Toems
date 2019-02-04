using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
     [Table("policy_history")]
    public class EntityPolicyHistory
    {
         [Key]
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         [Column("policy_history_id")]
         public int Id { get; set; }

         [Column("policy_id")]
         public int PolicyId { get; set; }

         [Column("computer_id")]
         public int ComputerId { get; set; }

         [Column("run_result")]
         public EnumPolicyHistory.RunResult Result { get; set; }

         [Column("policy_hash")]
         public string Hash { get; set; }

         [Column("run_time_utc")]
         public DateTime LastRunTime { get; set; }

         [Column("username")]
         public string User { get; set; }

         [NotMapped]
         public string PolicyGuid { get; set; }

         [NotMapped]
         public string ComputerName { get; set; }

    }
}
