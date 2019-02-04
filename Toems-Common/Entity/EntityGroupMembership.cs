using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("group_memberships")]
    public class EntityGroupMembership
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("group_membership_id")]
        public int Id { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

    }
}