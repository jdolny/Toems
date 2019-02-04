using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("bios_inventory")]
    public class EntityBiosInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("bios_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("serial_number")]
        public string SerialNumber { get; set; }

        [Column("version")]
        public string Version { get; set; }

        [Column("sm_bios_version")]
        public string SMBIOSBIOSVersion { get; set; }
    }
}
