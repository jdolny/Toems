using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("computer_process_history")]
    public class EntityComputerProcess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("computer_process_history_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("process_id")]
        public int ProcessId { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("start_time_utc")]
        public DateTime StartTimeUtc { get; set; }

        [Column("close_time_utc")]
        public DateTime CloseTimeUtc { get; set; }

    }
}