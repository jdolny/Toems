using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto.imageschemafe
{
    public class DtoHardDrive
    {
        public bool Active { get; set; }
        public string Boot { get; set; }
        public string Destination { get; set; }
        public string Guid { get; set; }
        public short Lbs { get; set; }
        public string Name { get; set; }
        public List<DtoPartition> Partitions { get; set; }
        public string Pbs { get; set; }
        public string Size { get; set; }
        public string Table { get; set; }
    }
}
