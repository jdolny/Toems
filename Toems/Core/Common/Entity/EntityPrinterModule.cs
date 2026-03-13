using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("printer_modules")]
    public class EntityPrinterModule
    {
        public EntityPrinterModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("printer_module_id", Order = 1)]
        public int Id { get; set; }

        [Column("printer_module_guid", Order = 2)]
        public string Guid { get; set; }

        [Column("printer_module_name", Order = 3)]
        public string Name { get; set; }

        [Column("printer_module_description", Order = 4)]
        public string Description { get; set; }

        [Column("printer_module_path", Order = 5)]
        public string NetworkPath { get; set; }

        [Column("printer_module_action", Order = 6)]
        public EnumPrinterModule.ActionType Action { get; set; }

        [Column("printer_module_setdefault", Order = 7)]
        public bool IsDefault { get; set; }

        [Column("printer_module_restartspooler", Order = 8)]
        public bool RestartSpooler { get; set; }

        [Column("is_archived", Order = 9)]
        public bool Archived { get; set; }

        [Column("wait_for_enumeration", Order = 10)]
        public bool WaitForEnumeration { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }
    }

    
}