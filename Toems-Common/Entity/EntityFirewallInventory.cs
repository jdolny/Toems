using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("firewall_inventory")]
    public class EntityFirewallInventory
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("firewall_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("domain_enabled")]
        public bool DomainEnabled { get; set; }

        [Column("private_enabled")]
        public bool PrivateEnabled { get; set; }

        [Column("public_enabled")]
        public bool PublicEnabled { get; set; }


     
    


    }
}