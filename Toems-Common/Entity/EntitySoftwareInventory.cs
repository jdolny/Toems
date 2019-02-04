using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("software_inventory")]
    public class EntitySoftwareInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("software_inventory_id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("version")]
        public string Version { get; set; }

         [Column("major")]
        public int Major { get; set; }

         [Column("minor")]
        public int Minor { get; set; }

         [Column("build")]
        public int Build { get; set; }

         [Column("revision")]
        public int Revision { get; set; }

    }
}
