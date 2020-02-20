namespace Toems_Common.Dto.imageschemabe
{
    public class DtoPartition
    {
        public bool Active { get; set; }
        public string CustomSize { get; set; }
        public string CustomSizeUnit { get; set; }
        public string EfiBootLoader { get; set; }
        public long End { get; set; }
        public bool ForceFixedSize { get; set; }
        public string FsId { get; set; }
        public string FsType { get; set; }
        public string Guid { get; set; }
        public string Number { get; set; }
        public string Prefix { get; set; }
        public long Size { get; set; }
        public long Start { get; set; }
        public string Type { get; set; }
        public long UsedMb { get; set; }
        public string Uuid { get; set; }
        public DtoVolumeGroup VolumeGroup { get; set; }
        public long VolumeSize { get; set; }
    }
}