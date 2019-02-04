using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("policy_hash_history")]
    public class EntityPolicyHashHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("policy_hash_history_id")]
        public int Id { get; set; }

        [Column("policy_id")]
        public int PolicyId { get; set; }

        [Column("policy_hash")]
        public string Hash { get; set; }

        [Column("policy_json")]
        public string Json { get; set; }

        [Column("modify_time_utc")]
        public DateTime ModifyTime { get; set; }

    }
}