using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("admin_settings")]
    public class EntitySetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("admin_setting_id")]
        public int Id { get; set; }

        [Column("admin_setting_name")]
        public string Name { get; set; }

        [Column("admin_setting_value")]
        public string Value { get; set; }
    }
}