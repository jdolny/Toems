using System.Collections.Generic;
using Toems_Common.Dto.client;
using Toems_Common.Enum;

namespace Toems_Common.Dto.exports
{
    public class DtoFileCopyModuleExport
    {
        public DtoFileCopyModuleExport()
        {
            UploadedFiles = new List<DtoUploadedFileExport>();
            ExternalFiles = new List<DtoExternalFileExport>();
        }

        public string DisplayName { get; set; }
        public string Destination { get; set; }
        public int Order { get; set; }
        public bool Unzip { get; set; }
        public bool Overwrite { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoScriptModuleExport Condition { get; set; }
        public List<DtoUploadedFileExport> UploadedFiles { get; set; }
        public List<DtoExternalFileExport> ExternalFiles { get; set; }
    }
}
