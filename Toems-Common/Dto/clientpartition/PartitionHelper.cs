namespace Toems_Common.Dto.clientpartition
{
    public class PartitionHelper
    {
        public bool IsDynamicSize { get; set; }
        public long MinSizeBlk { get; set; }
        public bool PartitionHasVolumeGroup { get; set; }
        public ClientVolumeGroupHelper VolumeGroupHelper { get; set; }
    }
}