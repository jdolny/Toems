using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
     [Table("custom_inventory")]
    public class EntityCustomInventory
    {
         [Key]
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         [Column("custom_inventory_id")]
         public int Id { get; set; }

         [Column("computer_id")]
         public int ComputerId { get; set; }

         [Column("script_module_id")]
         public int ScriptId { get; set; }

         [Column("value")]
         public string Value { get; set; }

         [NotMapped]
         public string ModuleGuid { get; set; }

    }
}
