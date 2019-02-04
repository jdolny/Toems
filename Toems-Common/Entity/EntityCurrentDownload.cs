using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("current_downloads")]
    public class EntityCurrentDownload
    {
        public EntityCurrentDownload()
        {
            LastRequestTimeLocal = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("current_download_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("last_request_time_local")]
        public DateTime LastRequestTimeLocal { get; set; }

        [Column("com_server")]
        public string ComServer { get; set; }

    }
}
