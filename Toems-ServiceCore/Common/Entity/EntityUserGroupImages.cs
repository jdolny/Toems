using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("toems_user_groups_image_acls")]
    public class EntityUserGroupImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toems_user_groups_image_acls_id")]
        public int Id { get; set; }

        [Column("user_group_id")]
        public int UserGroupId { get; set; }

        [Column("image_id")]
        public int ImageId { get; set; }

    }
}