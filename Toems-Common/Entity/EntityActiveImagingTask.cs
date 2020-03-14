using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("active_imaging_tasks")]
    public class EntityActiveImagingTask
    {
        public EntityActiveImagingTask()
        {
            Status = 0;
            QueuePosition = 0;
            LastUpdateTimeUTC = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("active_task_id")]
        public int Id { get; set; }

        [Column("task_arguments")]
        public string Arguments { get; set; }

        [Column("task_completed")]
        public string Completed { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("task_elapsed")]
        public string Elapsed { get; set; }


        [Column("multicast_id")]
        public int MulticastId { get; set; }

        [Column("task_partition")]
        public string Partition { get; set; }

        [Column("task_queue_position")]
        public int QueuePosition { get; set; }

        [Column("task_rate")]
        public string Rate { get; set; }

        [Column("task_remaining")]
        public string Remaining { get; set; }

        [Column("task_status")]
        public EnumTaskStatus.ImagingStatus Status { get; set; }

        [Column("task_type")]
        public string Type { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("com_server_id")]
        public int ComServerId { get; set; }

        [Column("last_update_time_utc")]
        public DateTime LastUpdateTimeUTC { get; set; }

        [Column("image_profile_id")]
        public int? ImageProfileId { get; set; }

        [NotMapped]
        public string Direction { get; set; }

    }

    [NotMapped]
    public class TaskWithComputer : EntityActiveImagingTask
    {
        public EntityComputer Computer { get; set; }
    }
}

