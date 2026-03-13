using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("logical_volume_inventory")]
    public class EntityLogicalVolumeInventory
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("logical_volume_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("drive")]
        public string Drive { get; set; }

        [Column("free_space_gb")]
        public int FreeSpaceGB { get; set; }

        [Column("free_space_percent")]
        public int FreeSpacePercent { get; set; }

        [Column("size_gb")]
        public int SizeGB { get; set; }

        
    


    }
}