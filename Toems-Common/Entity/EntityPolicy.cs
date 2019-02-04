using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("policies")]
    public class EntityPolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("policy_id")]
        public int Id { get; set; }

        [Column("policy_guid")]
        public string Guid { get; set; }

        [Column("policy_hash")]
        public string Hash { get; set; }

        [Column("policy_name")]
        public string Name { get; set; }

        [Column("policy_description")]
        public string Description { get; set; }

        [Column("policy_trigger")]
        public EnumPolicy.Trigger Trigger { get; set; }

        [Column("policy_frequency")]
        public EnumPolicy.Frequency Frequency { get; set; }

        [Column("policy_sub_frequency")]
        public int SubFrequency { get; set; }

        [Column("policy_start_date_utc")]
        public DateTime StartDate { get; set; }

        [Column("policy_completed_action")]
        public EnumPolicy.CompletedAction CompletedAction { get; set; }

        [Column("policy_run_inventory")]
        public EnumPolicy.InventoryAction RunInventory { get; set; }

        [Column("policy_run_login_tracker")]
        public bool RunLoginTracker { get; set; }

        [Column("policy_remove_cache")]
        public bool RemoveInstallCache { get; set; }

        [Column("policy_execution_type")]
        public EnumPolicy.ExecutionType ExecutionType { get; set; }

        [Column("policy_error_action")]
        public EnumPolicy.ErrorAction ErrorAction { get; set; }

        [Column("policy_log_level")]
        public EnumPolicy.LogLevel LogLevel { get; set; }

        [Column("policy_missed_action")]
        public EnumPolicy.FrequencyMissedAction MissedAction { get; set; }

        [Column("auto_archive_type")]
        public EnumPolicy.AutoArchiveType AutoArchiveType { get; set; }

        [Column("sub_auto_archive")]
        public string AutoArchiveSub { get; set; }

        [Column("is_archived")]
        public bool Archived { get; set; }

        [Column("skip_server_result")]
        public bool SkipServerResult { get; set; }

        [Column("run_application_monitor")]
        public bool RunApplicationMonitor { get; set; }

        [Column("window_start_schedule_id")]
        public int WindowStartScheduleId { get; set; }

        [Column("window_end_schedule_id")]
        public int WindowEndScheduleId { get; set; }

        [Column("policy_wu")]
        public EnumPolicy.WuType WuType { get; set; }

        [Column("policy_com_condition")]
        public EnumPolicy.PolicyComCondition PolicyComCondition { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }
    }

    [NotMapped]
    public class PolicyModules : EntityPolicy
    {
        public List<EntityPrinterModule> PrinterModules { get; set; }
        public List<EntitySoftwareModule> SoftwareModules { get; set; }
        public List<EntityFileCopyModule> FileCopyModules { get; set; }
        public List<EntityScriptModule> ScriptModules { get; set; }
        public List<EntityCommandModule> CommandModules { get; set; }
        public List<EntityWuModule> WuModules { get; set; }
        public int Order { get; set; }
    }

   
}