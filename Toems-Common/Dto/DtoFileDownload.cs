using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoFileDownload
    {
        public string FileName { get; set; }
        public string ModuleGuid { get; set; }
        public string Url { get; set; }
        public int ExternalDownloadId { get; set; }
        public string Sha256 { get; set; }
        public bool SyncWhenDone { get; set; }
    }
}
