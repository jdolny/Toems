using System.Collections.Generic;
using Toems_Common.Enum;

namespace Toems_Common.Dto.exports
{
    public class DtoCommandModuleExport
    {
        public DtoCommandModuleExport()
        {
            UploadedFiles = new List<DtoUploadedFileExport>();
            ExternalFiles = new List<DtoExternalFileExport>();
        }

        public string DisplayName { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
        public int Order { get; set; }
        public int Timeout { get; set; }
        public bool RedirectOutput { get; set; }
        public bool RedirectError { get; set; }
        public string SuccessCodes { get; set; }
        public List<DtoUploadedFileExport> UploadedFiles { get; set; }
        public List<DtoExternalFileExport> ExternalFiles { get; set; }
        public string WorkingDirectory { get; set; }       
        public string Description { get; set; }
        public string Guid { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoScriptModuleExport Condition { get; set; }
    }
}
