using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("external_downloads")]
    public class EntityExternalDownload
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("external_download_id")]
        public int Id { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("progress")]
        public string Progress { get; set; }

        [Column("status")]
        public EnumFileDownloader.DownloadStatus Status { get; set; }

        [Column("error_message")]
        public string ErrorMessage { get; set; }

        [Column("sha256_hash")]
        public string Sha256Hash { get; set; }

        [Column("date_downloaded_local")]
        public DateTime DateDownloaded { get; set; }

        [Column("module_guid")]
        public string ModuleGuid { get; set; }

        [Column("md5_hash")]
        public string Md5Hash { get; set; }

    }
}