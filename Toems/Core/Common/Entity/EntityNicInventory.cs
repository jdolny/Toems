using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("nic_inventory")]
    public class EntityNicInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("nic_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("nic_name")]
        public string Name { get; set; }

        [Column("nic_description")]
        public string Description { get; set; }

        [Column("nic_type")]
        public string Type { get; set; }

        [Column("nic_mac")]
        public string Mac { get; set; }

        [Column("nic_status")]
        public string Status { get; set; }

        [Column("nic_speed")]
        public long Speed { get; set; }

        [Column("nic_ips")]
        public string Ips { get; set; }

        [Column("nic_gateways")]
        public string Gateways { get; set; }
     
    }
}
