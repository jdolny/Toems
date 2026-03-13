using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("wie_builds")]
    public class EntityWieBuild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("wie_build_id")]
        public int Id { get; set; }

        [Column("wie_guid")]
        public string WieGuid { get; set; }

        [Column("datetime_started")]
        public DateTime StartTime { get; set; }

        [Column("datetime_end")]
        public DateTime EndTime { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("build_options")]
        public string BuildOptions { get; set; }

        [Column("pid")]
        public string Pid { get; set; }
    }
}