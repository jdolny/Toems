namespace Toems_Common.Dto
{
    public class DtoFreeSpace
    {
        public string dPPath { get; set; }
        public int freePercent { get; set; }
        public ulong freespace { get; set; }
        public ulong total { get; set; }
        public int usedPercent { get; set; }
    }
}
