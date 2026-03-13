using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Enum
{
    public class EnumManifestImport
    {
        public enum ImportStatus
        {
            Downloading = 0,
            Extracting = 1,
            Importing = 2,
            Complete = 3,
            Error = 4
        }
    }
}
