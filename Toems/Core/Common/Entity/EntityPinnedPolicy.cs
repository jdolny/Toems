using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("pinned_policies")]
    public class EntityPinnedPolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("pinned_policy_id")]
        public int Id { get; set; }

        [Column("policy_id")]
        public int PolicyId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

    }
}