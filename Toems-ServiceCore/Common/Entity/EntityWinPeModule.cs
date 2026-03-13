using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("winpe_modules")]
    public class EntityWinPeModule
    {
        public EntityWinPeModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("winpe_module_id")]
        public int Id { get; set; }

        [Column("winpe_module_guid")]
        public string Guid { get; set; }

        [Column("winpe_module_name")]
        public string Name { get; set; }

        [Column("winpe_module_description")]
        public string Description { get; set; }
        
        [Column("is_archived")]
        public bool Archived { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }
    }
}