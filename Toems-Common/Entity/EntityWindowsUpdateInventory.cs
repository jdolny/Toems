using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("wu_inventory")]
    public class EntityWindowsUpdateInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("wu_inventory_id")]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("update_id")]
        public string UpdateId { get; set; }

        [NotMapped]
        public string LastDeploymentChangeTime { get; set; }

        [NotMapped]
        public bool IsInstalled { get; set; }
    }
}
