using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("filecopy_modules")]
    public class EntityFileCopyModule
    {
        public EntityFileCopyModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("filecopy_module_id", Order = 1)]
        public int Id { get; set; }

        [Column("filecopy_module_guid", Order = 2)]
        public string Guid { get; set; }

        [Column("filecopy_module_name", Order = 3)]
        public string Name { get; set; }

        [Column("filecopy_module_description", Order = 4)]
        public string Description { get; set; }

        [Column("filecopy_module_destination", Order = 5)]
        public string Destination { get; set; }

        [Column("filecopy_module_decompress", Order = 6)]
        public bool DecompressAfterCopy { get; set; }
        
        [Column("is_archived", Order = 8)]
        public bool Archived { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }

        [Column("filecopy_overwrite")]
        public bool OverwriteExisting { get; set; }

        [Column("is_driver")]
        public bool IsDriver { get; set; }


    }
}