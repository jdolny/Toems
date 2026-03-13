using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("toec_deploy_threads")]
    public class EntityToecDeployThread
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toec_deploy_thread_id")]
        public int Id { get; set; }

        [Column("toec_deploy_thread_job_id")]
        public int JobId { get; set; }

        [Column("toec_deploy_thread_task_id")]
        public string TaskId { get; set; }

        [Column("toec_deploy_thread_datetime")]
        public DateTime? DateTimeUpdated { get; set; }
    }
}