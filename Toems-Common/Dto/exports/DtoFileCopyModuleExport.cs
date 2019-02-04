using System.Collections.Generic;
using Toems_Common.Dto.client;

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
        public string Description { get; set; }
        public string Guid { get; set; }

        public List<DtoUploadedFileExport> UploadedFiles { get; set; }
        public List<DtoExternalFileExport> ExternalFiles { get; set; }
    }
}
