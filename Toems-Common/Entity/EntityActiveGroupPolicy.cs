using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("active_group_policy")]
    public class EntityActiveGroupPolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("active_group_policy_id", Order = 1)]
        public int Id { get; set; }

        [Column("group_id", Order = 2)]
        public int GroupId { get; set; }

        [Column("json_string", Order = 3)]
        public string PolicyJson { get; set; }
    }
}