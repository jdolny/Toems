using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("computer_system_inventory")]
    public class EntityComputerSystemInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("computer_system_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("manufacturer")]
        public string Manufacturer { get; set; }

        [Column("model")]
        public string Model { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("domain")]
        public string Domain { get; set; }

        [Column("workgroup")]
        public string Workgroup { get; set; }

        [Column("memory")]
        public int Memory { get; set; }

        [Column("gpu")]
        public string Gpu { get; set; }

        [NotMapped]
        public UInt64 TotalPhysicalMemory { get; set; }
    }
}
