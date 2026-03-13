using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("toems_usergroup_memberships")]
    public class EntityUserGroupMembership
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("usergroup_membership_id")]
        public int Id { get; set; }

        [Column("user_group_id")]
        public int UserGroupId { get; set; }

        [Column("user_id")]
        public int ToemsUserId { get; set; }

    }
}