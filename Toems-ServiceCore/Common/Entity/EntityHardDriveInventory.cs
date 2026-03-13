using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("hdd_inventory")]
    public class EntityHardDriveInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("hdd_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("model")]
        public string Model { get; set; }

        [Column("firmware")]
        public string FirmwareRevision { get; set; }

        [Column("serial_number")]
        public string SerialNumber { get; set; }

        [Column("size")]
        public int SizeMb { get; set; }

        [Column("smart_status")]
        public string Status { get; set; }

        [NotMapped]
        public UInt64 Size { get; set; }
    }
}
