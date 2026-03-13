namespace Toems_Common.Dto.imageschemabe
{
    public class DtoVolumeGroup
    {
        public DtoLogicalVolume[] LogicalVolumes { get; set; }
        public string Name { get; set; }
        public string PhysicalVolume { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }
        public string Uuid { get; set; }
    }
}