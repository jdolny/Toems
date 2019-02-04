using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("printer_inventory")]
    public class EntityPrinterInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("printer_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("driver_name")]
        public string DriverName { get; set; }

        [Column("is_local")]
        public bool Local { get; set; }

        [Column("is_network")]
        public bool Network { get; set; }

        [Column("share_name")]
        public string ShareName { get; set; }

        [Column("system_name")]
        public string SystemName { get; set; }
    }
}
