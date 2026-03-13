using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("reset_requests")]
    public class EntityResetRequest
    {
        public EntityResetRequest()
        {
            RequestTime = DateTime.UtcNow;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("reset_request_id")]
        public int Id { get; set; }

        [Column("computer_name")]
        public string ComputerName { get; set; }

        [Column("ip_address")]
        public string IpAddress { get; set; }

        [Column("request_time_utc")]
        public DateTime RequestTime { get; set; }

        [Column("requestor_installation_id")]
        public string InstallationId { get; set; }
    }
}
