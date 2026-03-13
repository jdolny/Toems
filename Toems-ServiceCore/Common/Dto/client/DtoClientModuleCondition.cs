using System.Collections.Generic;
using Toems_Common.Enum;

namespace Toems_Common.Dto.client
{
    public class DtoClientModuleCondition
    {
        public DtoClientModuleCondition()
        {
            SuccessCodes = new List<string>();
            RunAs = string.Empty;
        }
        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public string Arguments { get; set; }
        public int Timeout { get; set; }
        public bool RedirectOutput { get; set; }
        public bool RedirectError { get; set; }
        public EnumScriptModule.ScriptType ScriptType { get; set; }
        public List<string> SuccessCodes { get; set; }
        public string WorkingDirectory { get; set; }
        public string RunAs { get; set; }
    }
}
