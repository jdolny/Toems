using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("com_server_cluster_servers")]
    public class EntityComServerClusterServer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("com_server_cluster_server_id")]
        public int Id { get; set; }

        [Column("com_server_cluster_id")]
        public int ComServerClusterId { get; set; }

        [Column("client_com_server_id")]
        public int ComServerId { get; set; }

        [Column("role")]
        public string Role { get; set; }

        [Column("is_imaging_server")]
        public bool IsImagingServer { get; set; }

        [Column("is_tftp_server")]
        public bool IsTftpServer { get; set; }

        [Column("is_multicast_server")]
        public bool IsMulticastServer { get; set; }

        [Column("is_em_server")]
        public bool IsEndpointManagementServer { get; set; }
    }
}