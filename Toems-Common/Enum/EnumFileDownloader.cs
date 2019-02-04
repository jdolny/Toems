using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Enum
{
    public class EnumFileDownloader
    {
        public enum DownloadStatus
        {
            Downloading = 0,
            CalculatingSha256 = 1,
            CalculatingMd5 = 2,
            Complete = 3,
            Error = 4,
            Queued = 5
        }
    }
}
