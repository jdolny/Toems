using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoDownloadConRequest
    {
        public string ComputerGuid { get; set; }
        public string ComServer { get; set; }
        public string ComputerName { get; set; }
    }
}
