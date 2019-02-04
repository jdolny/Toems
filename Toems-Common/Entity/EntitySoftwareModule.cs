using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("software_modules")]
    public class EntitySoftwareModule
    {
        public EntitySoftwareModule()
        {
            Guid = System.Guid.NewGuid().ToString();
            AdditionalArguments = "/q /norestart";
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("software_module_id", Order = 1)]
        public int Id { get; set; }

        [Column("software_module_guid", Order = 2)]
        public string Guid { get; set; }

        [Column("software_module_name", Order = 3)]
        public string Name { get; set; }

        [Column("software_module_description", Order = 4)]
        public string Description { get; set; }

        [Column("software_module_type", Order = 5)]
        public EnumSoftwareModule.MsiInstallType InstallType { get; set; }

        [Column("software_module_command", Order = 6)]
        public string Command { get; set; }

        [Column("software_module_arguments", Order = 7)]
        public string Arguments { get; set; }

        [Column("software_module_addarguments", Order = 8)]
        public string AdditionalArguments { get; set; }

        [Column("software_module_timeout", Order = 9)]
        public int Timeout { get; set; }

        [Column("redirect_stdout", Order = 11)]
        public bool RedirectStdOut { get; set; }

        [Column("redirect_stderror", Order = 12)]
        public bool RedirectStdError { get; set; }

        [Column("success_codes", Order = 13)]
        public string SuccessCodes { get; set; }

        [Column("is_archived", Order = 14)]
        public bool Archived { get; set; }

        [Column("impersonation_id")]
        public int ImpersonationId { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }
    }
}