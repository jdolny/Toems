using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("toems_users")]
    public class EntityToemsUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toems_user_id", Order = 1)]
        public int Id { get; set; }

        [Column("toems_username", Order = 2)]
        public string Name { get; set; }
       
        [Column("toems_user_pwd", Order = 3)]
        public string Password { get; set; }

        [Column("toems_user_salt", Order = 4)]
        public string Salt { get; set; }

        [Column("toems_user_role", Order = 5)]
        public string Membership { get; set; }

        [Column("toems_user_email", Order = 6)]
        public string Email { get; set; }

        [Column("toems_user_is_ldap", Order = 14)]
        public int IsLdapUser { get; set; }

        [Column("toems_usergroup_id", Order = 15)]
        public int UserGroupId { get; set; }

        [Column("toems_theme")]
        public string Theme { get; set; }

        [Column("imaging_token")]
        public string ImagingToken { get; set; }


    }

    [NotMapped]
    public class UserWithUserGroup : EntityToemsUser
    {
        public EntityToemsUserGroup UserGroup { get; set; }
    }
}