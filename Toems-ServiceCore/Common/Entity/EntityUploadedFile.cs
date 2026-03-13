using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("uploaded_files")]
    public class EntityUploadedFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("uploaded_file_id", Order = 1)]
        public int Id { get; set; }

        [Column("module_guid", Order = 2)]
        public string Guid { get; set; }

        [Column("uploaded_file_hash", Order = 3)]
        public string Hash { get; set; }

        [Column("uploaded_file_name", Order = 4)]
        public string Name { get; set; }

        [Column("uploaded_time_utc", Order = 5)]
        public DateTime DateUploaded { get; set; }   

    }
}