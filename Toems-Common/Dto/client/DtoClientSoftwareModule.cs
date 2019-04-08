using System.Collections.Generic;
using Toems_Common.Enum;

namespace Toems_Common.Dto.client
{
    public class DtoClientSoftwareModule
    {
        public DtoClientSoftwareModule()
        {
            Files = new List<DtoClientFileHash>();
            SuccessCodes = new List<string>();
            RunAs = string.Empty;
            Condition = new DtoClientModuleCondition();
        }
        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
        public int Order { get; set; }
        public int Timeout { get; set; }
        public bool RedirectOutput { get; set; }
        public bool RedirectError { get; set; }
        public EnumSoftwareModule.MsiInstallType InstallType { get; set; }
        public List<DtoClientFileHash> Files { get; set; }
        public List<string> SuccessCodes { get; set; }
        public string RunAs { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoClientModuleCondition Condition { get; set; }
    }
}
