using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("winget_locale_manifests")]
    public class EntityWingetLocaleManifest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("winget_locale_manifest_id")]
        public int Id { get; set; }

        [Column("package_identifier")]
        public string PackageIdentifier { get; set; }

        [Column("package_version")]
        public string PackageVersion { get; set; }

        [Column("package_locale")]
        public string Locale { get; set; }

        [Column("publisher")]
        public string Publisher { get; set; }

        [Column("publisher_url")]
        public string PublisherUrl { get; set; }

        [Column("Package_name")]
        public string PackageName { get; set; }

        [Column("package_url")]
        public string PackageUrl { get; set; }

        [Column("license")]
        public string License { get; set; }

        [Column("short_description")]
        public string ShortDescription { get; set; }

        [Column("manifest_type")]
        public string ManifestType { get; set; }

        [Column("tags")]
        public string Tags { get; set; }

        [Column("moniker")]
        public string Moniker { get; set; }

        [Column("major")]
        public int Major { get; set; }
        [Column("minor")]
        public int Minor { get; set; }
        [Column("build")]
        public int Build { get; set; }
        [Column("revision")]
        public int Revision { get; set; }

        [NotMapped]
        public string Scope { get; set; }



    }
}