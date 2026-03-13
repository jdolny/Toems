using System.Collections.Generic;
using Toems_Common.Enum;

namespace Toems_Common.Dto.client
{
    public class DtoClientWingetModule
    {
        public DtoClientWingetModule()
        {
            RunAs = string.Empty;
            Condition = new DtoClientModuleCondition();
        }

        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }
        public string Arguments { get; set; }
        public string Override { get; set; }
        public bool InstallLatest { get; set; }
        public bool KeepUpdated { get; set; }
        public EnumWingetInstallType.WingetInstallType InstallType { get; set; }
        public int Order { get; set; }
        public int Timeout { get; set; }
        public bool RedirectOutput { get; set; }
        public bool RedirectError { get; set; }
        public string RunAs { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoClientModuleCondition Condition { get; set; }

        
    }
}
