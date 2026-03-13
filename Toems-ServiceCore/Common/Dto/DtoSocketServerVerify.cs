using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoSocketServerVerify
    {
        public string Timestamp { get; set; }
        public string nOnce { get; set; }
        public string signature { get; set; }
    }
}
