using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("toems_user_groups")]
    public class EntityToemsUserGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toems_user_group_id", Order = 1)]
        public int Id { get; set; }

        [Column("toems_user_group_name", Order = 2)]
        public string Name { get; set; }

        [Column("toems_user_group_role", Order = 3)]
        public string Membership { get; set; }

        [Column("toems_user_group_ldap", Order = 4)]
        public int IsLdapGroup { get; set; }

        [Column("toems_user_group_ldapname", Order = 5)]
        public string GroupLdapName { get; set; }

        [Column("enable_image_acls")]
        public bool EnableImageAcls { get; set; }

        [Column("enable_computergroup_acls")]
        public bool EnableComputerGroupAcls { get; set; }

       }
}