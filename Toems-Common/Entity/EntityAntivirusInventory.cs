using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("antivirus_inventory")]
    public class EntityAntivirusInventory
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("antivirus_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("provider")]
        public string Provider { get; set; }

        [Column("rt_scanner")]
        public string RealtimeScanner { get; set; }

        [Column("definition_status")]
        public string DefinitionStatus { get; set; }

        [Column("product_state")]
        public int ProductState { get; set; }

    


    }
}