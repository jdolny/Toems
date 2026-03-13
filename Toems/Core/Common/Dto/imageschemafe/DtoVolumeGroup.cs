using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto.imageschemafe
{
    public class DtoVolumeGroup
    {
        public DtoLogicalVolume[] LogicalVolumes { get; set; }
        public string Name { get; set; }
        public string PhysicalVolume { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Uuid { get; set; }
    }
}
