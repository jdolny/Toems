using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoIsoGenOptions
    {
        public string arguments { get; set; }
        public string bootImage { get; set; }
        public string kernel { get; set; }
        public int clusterId { get; set; }
    }
}
