using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Enum
{
    public class EnumSoftwareAsset
    {
        public enum SoftwareMatchType
        {
            Name = 0,
            Name_FullVersion = 1,
            Name_MajorVersion = 2,
            Name_MajorVersion_MinorVersion = 3,
            Name_MajorVersion_MinorVersion_Build = 4
        }

    }
}
