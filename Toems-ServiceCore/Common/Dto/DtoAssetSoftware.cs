using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Enum;

namespace Toems_Common.Dto
{
    public class DtoAssetSoftware
    {
        public int SoftwareAssetSoftwareId { get; set; }
        public int SoftwareId { get; set; }
        public EnumSoftwareAsset.SoftwareMatchType MatchType { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
    }
}
