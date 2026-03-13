using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("com_server_clusters")]
    public class EntityComServerCluster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("com_server_cluster_id")]
        public int Id { get; set; }

        [Column("com_server_cluster_name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; }
    }
}