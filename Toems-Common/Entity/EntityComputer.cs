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
        [Column("computer_id", Order = 1)]
        public int Id { get; set; }

        [Column("computer_name", Order = 2)]
        public string Name { get; set; }

        [Column("computer_guid", Order = 3)]
        public string Guid { get; set; }

        [Column("provision_status", Order = 4)]
        public EnumProvisionStatus.Status ProvisionStatus { get; set; }

        [Column("certificate_id", Order = 5)]
        public int CertificateId { get; set; }

        [Column("symmetric_key_enc", Order = 6)]
        public string SymmKeyEncrypted { get; set; }

        [Column("ad_guid", Order = 7)]
        public string AdGuid { get; set; }

        [Column("installation_id", Order = 8)]
        public string InstallationId { get; set; }

        [Column("provisioned_time_local", Order = 9)]
        public DateTime ProvisionedTime { get; set; }

        [Column("last_checkin_time_local", Order = 10)]
        public DateTime LastCheckinTime { get; set; }

        [Column("last_ip", Order = 11)]
        public string LastIp { get; set; }

        [Column("is_ad_sync", Order = 12)]
        public bool IsAdSync { get; set; }

        [Column("client_version", Order = 13)]
        public string ClientVersion { get; set; }

        [Column("last_inventory_time_local", Order = 14)]
        public DateTime LastInventoryTime { get; set; }

        [Column("push_url", Order = 15)]
        public string PushUrl { get; set; }

        [Column("ad_disabled", Order = 16)]
        public bool AdDisabled { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }

        [Column("remote_access_id")]
        public string RemoteAccessId { get; set; }

    }

    [NotMapped]
    public class ComputerWithCategories : EntityComputer
    {
        public List<EntityComputerCategory> ComputerCategories { get; set; }
    }
}