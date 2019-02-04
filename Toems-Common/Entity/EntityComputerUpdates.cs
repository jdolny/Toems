using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("computer_updates")]
    public class EntityComputerUpdates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("computer_update_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("wu_inventory_id")]
        public int UpdateId { get; set; }

        [Column("install_date")]
        public string LastDeploymentChangeTime { get; set; }

        [Column("is_installed")]
        public bool IsInstalled { get; set; }

    }
}