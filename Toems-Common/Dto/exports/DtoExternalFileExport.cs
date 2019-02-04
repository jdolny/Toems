using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto.exports
{
    public class DtoExternalFileExport
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public string Sha256Hash { get; set; }
        public string ModuleGuid { get; set; }
    }
}
