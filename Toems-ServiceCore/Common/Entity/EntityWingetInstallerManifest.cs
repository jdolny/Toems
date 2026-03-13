using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("winget_installer_manifests")]
    public class EntityWingetInstallerManifest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("winget_installer_manifest_id")]
        public int Id { get; set; }

        [Column("package_identifier")]
        public string PackageIdentifier { get; set; }

        [Column("package_version")]
        public string PackageVersion { get; set; }

        [Column("scope")]
        public string Scope { get; set; }

    }
}