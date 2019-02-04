using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("processor_inventory")]
    public class EntityProcessorInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("processor_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }


        [Column("name")]
        public string Name { get; set; }

        [Column("clock_speed")]
        public int Speed { get; set; }

        [Column("cores")]
        public int Cores { get; set; }

        [NotMapped]
        public UInt32 MaxClockSpeed { get; set; }

        [NotMapped]  
        public UInt32 NumberOfCores { get; set; }
    }
}
