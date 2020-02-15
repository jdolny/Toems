using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("sysprep_modules")]
    public class EntitySysprepModule
    {
        public EntitySysprepModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("sysprep_module_id")]
        public int Id { get; set; }

        [Column("sysprep_module_close")]
        public string ClosingTag { get; set; }

        [Column("sysprep_module_contents")]
        public string Contents { get; set; }

        [Column("sysprep_module_description")]
        public string Description { get; set; }   

        [Column("sysprep_module_name")]
        public string Name { get; set; }

        [Column("sysprep_module_open")]
        public string OpeningTag { get; set; }

        [Column("sysprep_module_guid")]
        public string Guid { get; set; }

        [Column("is_archived")]
        public bool Archived { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }
    }
}
