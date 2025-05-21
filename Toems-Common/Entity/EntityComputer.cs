using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("computers")]
    public class EntityComputer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("computer_id")] public int Id { get; set; }
        [Column("computer_name")] public string Name { get; set; } = null;
        [Column("computer_guid")] public string Guid { get; set; } = null;
        [Column("provision_status")] public EnumProvisionStatus.Status ProvisionStatus { get; set; } = EnumProvisionStatus.Status.NotStarted;
        [Column("certificate_id")] public int CertificateId { get; set; } = -1;
        [Column("symmetric_key_enc")] public string SymmKeyEncrypted { get; set; } = null;
        [Column("ad_guid")] public string AdGuid { get; set; } = null;
        [Column("installation_id")] public string InstallationId { get; set; } = null;
        [Column("provisioned_time_local")] public DateTime ProvisionedTime { get; set; } = DateTime.Parse("0001-01-01 00:00:00");
        [Column("last_checkin_time_local")] public DateTime LastCheckinTime { get; set; } = DateTime.Parse("0001-01-01 00:00:00");
        [Column("last_ip")] public string LastIp { get; set; } = null;
        [Column("is_ad_sync")] public bool IsAdSync { get; set; } = false;
        [Column("client_version")] public string ClientVersion { get; set; } = null;
        [Column("last_inventory_time_local")] public DateTime LastInventoryTime { get; set; } = DateTime.Parse("0001-01-01 00:00:00");
        [Column("push_url")] public string PushUrl { get; set; } = null;
        [Column("ad_disabled")] public bool AdDisabled { get; set; } = false;
        [Column("datetime_archived_local")] public DateTime? ArchiveDateTime { get; set; } = null;
        [Column("remote_access_id")] public string RemoteAccessId { get; set; } = null;
        [Column("imaging_mac")] public string ImagingMac { get; set; } = null;
        [Column("imaging_client_id")] public string ImagingClientId { get; set; } = null;
        [Column("image_id")] public int ImageId { get; set; } = -1;
        [Column("image_profile_id")] public int ImageProfileId { get; set; } = -1;
        [Column("hardware_uuid")] public string UUID { get; set; } = null;
        [Column("last_socket_result")] public string LastSocketResult { get; set; } = null;
        [Column("pxe_ip_address")] public string PxeIpAddress { get; set; } = null;
        [Column("pxe_netmask")] public string PxeNetmask { get; set; } = null;
        [Column("pxe_gateway")] public string PxeGateway { get; set; } = null;
        [Column("pxe_dns")] public string PxeDns { get; set; } = null;
        [Column("winpe_module_id")] public int WinPeModuleId { get; set; } = -1;
        [Column("computer_description")] public string Description { get; set; } = null;

        [NotMapped]
        public string LastLoggedInUser { get; set; }
        [NotMapped]
        public string Status { get; set; }
        [NotMapped]
        public string CurrentImage { get; set; }
        [NotMapped]
        public string Manufacturer { get; set; }
        [NotMapped]
        public string Model { get; set; }
        [NotMapped]
        public string OsName { get; set; }
        [NotMapped]
        public string OsVersion { get; set; }
        [NotMapped]
        public string OsBuild { get; set; }
        [NotMapped]
        public string Domain { get; set; }
    }

    [NotMapped]
    public class ComputerWithCategories : EntityComputer
    {
        public List<EntityComputerCategory> ComputerCategories { get; set; }
    }

    [NotMapped]
    public class ComputerWithImage : EntityComputer
    {
        public EntityImage Image { get; set; }
        public EntityImageProfile ImageProfile { get; set; }
    }
}