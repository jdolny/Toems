using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("client_com_servers")]
    public class EntityClientComServer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("client_com_server_id")]
        public int Id { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("replicate_storage")]
        public bool ReplicateStorage { get; set; }

        [Column("is_imaging_server")]
        public bool IsImagingServer { get; set; }

        [Column("is_tftp_server")]
        public bool IsTftpServer { get; set; }

        [Column("is_multicast_server")]
        public bool IsMulticastServer { get; set; }

        [Column("unique_id")]
        public string UniqueId { get; set; }

        [Column("is_em_server")]
        public bool IsEndpointManagementServer { get; set; }

        [Column("local_storage_path")]
        public string LocalStoragePath { get; set; }

        [Column("em_max_bps")]
        public int EmMaxBps { get; set; }

        [Column("em_max_clients")]
        public int EmMaxClients { get; set; }

        [Column("imaging_max_bps")]
        public int ImagingMaxBps { get; set; }

        [Column("imaging_max_clients")]
        public int ImagingMaxClients { get; set; }

        [Column("replication_rate_ipg")]
        public int ReplicationRateIpg { get; set; }

        [Column("tftp_path")]
        public string TftpPath { get; set; }
        
        [Column("tftp_interface_ip")]
        public string TftpInterfaceIp { get; set; }

        [Column ("multicast_interface_ip")]
        public string MulticastInterfaceIp { get; set; }

        [Column ("multicast_start_port")]
        public int MulticastStartPort { get; set; }

        [Column ("multicast_end_port")]
        public int MulticastEndPort { get; set; }

        [Column ("multicast_send_arguments")]
        public string MulticastSenderArguments { get; set; }

        [Column("multicast_rec_arguments")]
        public string MulticastReceiverArguments { get; set; }

        [Column("decompress_image")]
        public string DecompressImageOn { get; set; }

        [Column("tftp_info_server")]
        public bool IsTftpInfoServer { get; set; }

        [Column("image_info_server")]
        public bool IsImageInfoServer { get; set; }

    }
}