using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("os_inventory")]
    public class EntityOsInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("os_inventory_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("name")]
        public string Caption { get; set; }

        [Column("version")]
        public string Version { get; set; }

        [Column("build")]
        public string BuildNumber { get; set; }

        [Column("arch")]
        public string OSArchitecture { get; set; }

        [Column("sp_major")]
        public int SpMajor { get; set; }

        [Column("sp_minor")]
        public int SpMinor { get; set; }

        [Column("release_id")]
        public string ReleaseId { get; set; }

        [Column("uac_status")]
        public string UacStatus { get; set; }

        [Column("local_time_zone")]
        public string LocalTimeZone { get; set; }

        [Column("latitude")]
        public string Latitude { get; set; }

        [Column("longitude")]
        public string Longitude { get; set; }

        [Column("location_enabled")]
        public bool LocationEnabled { get; set; }

        [Column("last_location_update_utc")]
        public DateTime LastLocationUpdateUtc { get; set; }

        [Column("update_server")]
        public string UpdateServer { get; set; }

        [Column("update_server_target_group")]
        public string SUStargetGroup { get; set; }



        [NotMapped]
        public UInt16 ServicePackMajorVersion { get; set; }

        [NotMapped]
        public UInt16 ServicePackMinorVersion { get; set; }
    }
}
