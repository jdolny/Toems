using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("toems_user_groups_computer_acls")]
    public class EntityUserGroupComputerGroups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toems_user_groups_computer_acls_id")]
        public int Id { get; set; }

        [Column("user_group_id")]
        public int UserGroupId { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

    }
}