using System.Collections.Generic;

namespace Toems_Common.Dto.clientimaging
{
    public class FileFolderCopySchema
    {
        public string Count { get; set; }
        public List<FileFolderCopy> FilesAndFolders { get; set; }
    }
}