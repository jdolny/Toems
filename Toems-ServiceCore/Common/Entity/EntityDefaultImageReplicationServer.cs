using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("default_image_replication_servers")]
    public class EntityDefaultImageReplicationServer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("default_image_replication_server_id")]
        public int Id { get; set; }

        [Column("com_server_id")]
        public int ComServerId { get; set; }

    }
}