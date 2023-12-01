using System.Collections.Generic;
using Toems_Common.Enum;

namespace Toems_Common.Dto.exports
{
    public class DtoWingetModuleExport
    {
        public DtoWingetModuleExport()
        {

        }

        public string DisplayName { get; set; }
        public string PackageIdentifier { get; set; }
        public string PackageVersion { get; set; }
        public bool InstallLatest { get; set; }
        public bool KeepUpdated { get; set; }
        public string Arguments { get; set; }
        public string Override { get; set; }
        public int Order { get; set; }
        public int Timeout { get; set; }
        public bool RedirectOutput { get; set; }
        public bool RedirectError { get; set; }

        public string Description { get; set; }
        public string Guid { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoScriptModuleExport Condition { get; set; }
    }
}
