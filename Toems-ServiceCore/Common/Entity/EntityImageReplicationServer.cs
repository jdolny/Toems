using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("image_replication_servers")]
    public class EntityImageReplicationServer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("image_replication_server_id")]
        public int Id { get; set; }

        [Column("image_id")]
        public int ImageId { get; set; }

        [Column("com_server_id")]
        public int ComServerId { get; set; }

    }
}