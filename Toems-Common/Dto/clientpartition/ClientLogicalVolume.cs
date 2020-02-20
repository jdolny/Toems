namespace Toems_Common.Dto.clientpartition
{
    public class ClientLogicalVolume
    {
        public string FsType { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public bool SizeIsDynamic { get; set; }
        public string Uuid { get; set; }
        public string Vg { get; set; }
        public string VgUuid { get; set; }
    }
}