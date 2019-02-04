using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoProvisionHardware
    {
        public DtoProvisionHardware()
        {
            Macs = new List<string>();
        }

        public string Processor { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public int Memory { get; set; }
        public List<string> Macs { get; set; }

    }
}
