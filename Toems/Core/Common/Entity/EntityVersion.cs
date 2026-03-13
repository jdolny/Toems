using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("toems_version")]
    public class EntityVersion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toems_version_id")]
        public int Id { get; set; }

        [Column("expected_app_version")]
        public string ExpectedAppVersion { get; set; }

        [Column("expected_toecapi_version")]
        public string ExpectedToecApiVersion { get; set; }

        [Column("database_version")]
        public string DatabaseVersion { get; set; }

        [Column("first_run_completed")]
        public int FirstRunCompleted { get; set; }

        [Column("latest_client_version")]
        public string LatestClientVersion { get; set; }

        
    }
}