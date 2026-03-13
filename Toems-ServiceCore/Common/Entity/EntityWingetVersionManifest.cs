using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("winget_version_manifests")]
    public class EntityWingetVersionManifest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("winget_version_manifest_id")]
        public int Id { get; set; }

        [Column("package_identifier")]
        public string PackageIdentifier { get; set; }

        [Column("package_version")]
        public string PackageVersion { get; set; }

        [Column("default_locale")]
        public string DefaultLocale { get; set; }

    }
}