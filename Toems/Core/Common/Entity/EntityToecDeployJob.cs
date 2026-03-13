using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("toec_deploy_jobs")]
    public class EntityToecDeployJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toec_deploy_job_id")]
        public int Id { get; set; }

        [Column("toec_deploy_job_domain")]
        public string Domain { get; set; }

        [Column("toec_deploy_job_username")]
        public string Username { get; set; }

        [Column("toec_deploy_job_password_encrypted")]
        public string PasswordEncrypted { get; set; }

        [Column("toec_deploy_target_list_id")]
        public int TargetListId { get; set; }
        
        [Column("toec_deploy_exclusion_list_id")]
        public int ExclusionListId { get; set; }
        
        [Column("toec_deploy_job_type")]
        public EnumToecDeployJob.JobType JobType { get; set; }
        
        [Column("toec_deploy_run_mode")]
        public EnumToecDeployJob.RunMode RunMode { get; set; }
                
        [Column("toec_deploy_job_is_enabled")]
        public bool Enabled { get; set; }

        [Column("toec_deploy_job_name")]
        public string Name { get; set; }



    }
}