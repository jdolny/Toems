using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("process_inventory")]
    public class EntityProcessInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("process_inventory_id")]
        public int Id { get; set; }

        [Column("process_name")]
        public string Name { get; set; }

        [Column("process_path")]
        public string Path { get; set; }
    }
}
