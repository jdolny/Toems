using System.Collections.Generic;

namespace Toems_Common.Dto.clientimaging
{
    public class VolumeGroup
    {
        public int LogicalVolumeCount { get; set; }
        public List<LogicalVolume> LogicalVolumes { get; set; }
        public string Name { get; set; }
    }
}