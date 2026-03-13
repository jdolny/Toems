using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("winget_manifest_downloads")]
    public class EntityWingetManifestDownload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("winget_manifest_download_id")]
        public int Id { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("progress")]
        public string Progress { get; set; }

        [Column("status")]
        public EnumManifestImport.ImportStatus Status { get; set; }

        [Column("error_message")]
        public string ErrorMessage { get; set; }

        [Column("date_downloaded_local")]
        public DateTime DateDownloaded { get; set; }

    }
}