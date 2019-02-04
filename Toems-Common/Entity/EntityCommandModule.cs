using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("command_modules")]
    public class EntityCommandModule
    {
        public EntityCommandModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("command_module_id", Order = 1)]
        public int Id { get; set; }

        [Column("command_module_guid", Order = 2)]
        public string Guid { get; set; }

        [Column("command_module_name", Order = 3)]
        public string Name { get; set; }

        [Column("command_module_description", Order = 4)]
        public string Description { get; set; }

        [Column("command_module_command", Order = 5)]
        public string Command { get; set; }

        [Column("command_module_arguments", Order = 6)]
        public string Arguments { get; set; }

        [Column("command_module_timeout", Order = 7)]
        public int Timeout { get; set; }

        [Column("command_module_workingdir", Order = 8)]
        public string WorkingDirectory { get; set; }

        [Column("redirect_stdout", Order = 9)]
        public bool RedirectStdOut { get; set; }

        [Column("redirect_stderror", Order = 10)]
        public bool RedirectStdError { get; set; }

        [Column("success_codes", Order = 11)]
        public string SuccessCodes { get; set; }

        [Column("is_archived", Order = 12)]
        public bool Archived { get; set; }

        [Column("impersonation_id")]
        public int ImpersonationId { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }


    }
}