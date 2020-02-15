using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("custom_boot_menu")]
    public class EntityCustomBootMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("custom_boot_menu_id")]
        public int Id { get; set; }

        [Column("custom_boot_menu_name")]
        public string Name { get; set; }

        [Column("custom_boot_menu_description")]
        public string Description { get; set; }

        [Column("custom_boot_menu_type")]
        public string Type { get; set; }

        [Column("custom_boot_menu_order")]
        public int Order { get; set; }

        [Column("custom_boot_menu_content")]
        public string Content { get; set; }

        [Column("custom_boot_menu_is_active")]
        public bool IsActive { get; set; }

        [Column("custom_boot_menu_is_default")]
        public bool IsDefault { get; set; }

    }
}