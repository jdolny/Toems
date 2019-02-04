using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("bitlocker_inventory")]
    public class EntityBitlockerInventory
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("bitlocker_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("drive_letter")]
        public string DriveLetter { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [NotMapped]
        public UInt32 ProtectionStatus { get; set; }
    


    }
}