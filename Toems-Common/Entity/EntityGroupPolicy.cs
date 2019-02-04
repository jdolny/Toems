using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("group_policies")]
    public class EntityGroupPolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("group_policy_id", Order = 1)]
        public int Id { get; set; }

        [Column("group_id", Order = 2)]
        public int GroupId { get; set; }

        [Column("policy_id", Order = 3)]
        public int PolicyId { get; set; }

        [Column("policy_order", Order = 4)]
        public int PolicyOrder { get; set; }
    }

    [NotMapped]
    public class GroupPolicyDetailed : EntityGroupPolicy
    {
        public EntityPolicy Policy { get; set; }
    }
}
