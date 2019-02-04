using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("toems_usergroup_rights")]
    public class EntityUserGroupRight
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toems_usergroup_right_id", Order = 1)]
        public int Id { get; set; }

        [Column("usergroup_id", Order = 2)]
        public int UserGroupId { get; set; }

        [Column("usergroup_right", Order = 3)]
        public string Right { get; set; }

       
    }
}