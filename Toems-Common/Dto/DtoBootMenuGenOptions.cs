using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoBootMenuGenOptions
    {
        public string AddPwd { get; set; }
        public string BootImage { get; set; }
        public string DebugPwd { get; set; }
        public string DiagPwd { get; set; }
        public string GrubPassword { get; set; }
        public string GrubUserName { get; set; }
        public string Kernel { get; set; }
        public string OndPwd { get; set; }
        public string Type { get; set; }
        public string BiosKernel { get; set; }
        public string BiosBootImage { get; set; }
        public string Efi32Kernel { get; set; }
        public string Efi32BootImage { get; set; }
        public string Efi64Kernel { get; set; }
        public string Efi64BootImage { get; set; }
    }
}
