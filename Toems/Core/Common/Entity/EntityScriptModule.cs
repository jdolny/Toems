using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("script_modules")]
    public class EntityScriptModule
    {
        public EntityScriptModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("script_module_id", Order = 1)]
        public int Id { get; set; }

        [Column("script_module_guid", Order = 2)]
        public string Guid { get; set; }

        [Column("script_module_name", Order = 3)]
        public string Name { get; set; }

        [Column("script_module_description", Order = 4)]
        public string Description { get; set; }

        [Column("script_module_type", Order = 5)]
        public EnumScriptModule.ScriptType ScriptType { get; set; }

        [Column("script_module_arguments", Order = 6)]
        public string Arguments { get; set; }

        [Column("script_module_contents", Order = 7)]
        public string ScriptContents { get; set; }
        
        [Column("script_module_timeout", Order = 8)]
        public int Timeout { get; set; }

        [Column("script_module_workingdir", Order = 9)]
        public string WorkingDirectory { get; set; }

        [Column("redirect_stdout", Order = 10)]
        public bool RedirectStdOut { get; set; }

        [Column("redirect_stderror", Order = 11)]
        public bool RedirectStdError { get; set; }

        [Column("success_codes", Order = 12)]
        public string SuccessCodes { get; set; }

        [Column("add_inventory_collection", Order = 13)]
        public bool AddInventoryCollection { get; set; }

        [Column("use_as_condition", Order = 14)]
        public bool IsCondition { get; set; }

        [Column("is_archived", Order = 15)]
        public bool Archived { get; set; }

        [Column("impersonation_id")]
        public int ImpersonationId { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }


    }
}