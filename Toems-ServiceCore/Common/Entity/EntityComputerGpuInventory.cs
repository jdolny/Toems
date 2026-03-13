using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("computer_gpu_inventory")]
    public class EntityComputerGpuInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("computer_gpu_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("computer_gpu_name")]
        public string Name { get; set; }

        [Column("computer_gpu_ram")]
        public int Memory { get; set; }

        [NotMapped]
        public UInt64 AdapterRam { get; set; }

    }
}
