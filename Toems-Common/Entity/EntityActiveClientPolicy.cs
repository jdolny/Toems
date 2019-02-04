using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("active_client_policies")]
    public class EntityActiveClientPolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("active_client_policy_id", Order = 1)]
        public int Id { get; set; }

        [Column("policy_id", Order = 2)]
        public int PolicyId { get; set; }

        [Column("json_string", Order = 3)]
        public string PolicyJson { get; set; }
    }
}