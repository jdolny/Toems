using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto.imageschemabe;

namespace Toems_Common.Dto
{
    public class DtoMulticastArgs
    {
        public string clientCount { get; set; }
        public string Environment { get; set; }
        public string ExtraArgs { get; set; }
        public string groupName { get; set; }
        public string ImageName { get; set; }
        public string Port { get; set; }
        public DtoImageSchema schema { get; set; }
    }
}
