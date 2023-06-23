using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("winget_modules")]
    public class EntityWingetModule
    {
        public EntityWingetModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("winget_module_id")]
        public int Id { get; set; }

        [Column("winget_module_guid")]
        public string Guid { get; set; }

        [Column("winget_module_name")]
        public string Name { get; set; }

        [Column("winget_module_description")]
        public string Description { get; set; }

        [Column("winget_module_arguments")]
        public string Arguments { get; set; }

        [Column("winget_module_override")]
        public string Override { get; set; }

        [Column("winget_package_id")]
        public string PackageId { get; set; }

        [Column("winget_package_version")]
        public string PackageVersion { get; set; }

        [Column("winget_install_type")]
        public EnumWingetInstallType.WingetInstallType InstallType { get; set; }

        [Column("winget_keep_updated")]
        public bool KeepUpdated { get; set; }

        [Column("is_archived")]
        public bool Archived { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }

        [Column("winget_module_timeout")]
        public int Timeout { get; set; }

        [Column("redirect_stdout")]
        public bool RedirectStdOut { get; set; }

        [Column("redirect_stderror")]
        public bool RedirectStdError { get; set; }

        [Column("impersonation_id")]
        public int ImpersonationId { get; set; }


    }
}