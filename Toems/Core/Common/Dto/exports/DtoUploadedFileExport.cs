using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto.exports
{
    public class DtoUploadedFileExport
    {
        public string FileName { get; set; }
        public string Md5Hash { get; set; }
        public string ModuleGuid { get; set; }
    }
}
