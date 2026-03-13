using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoRemotelyConnectionInfo
    {
        public string DeviceID { get; set; }
        public string Host { get; set; }
        public string OrganizationID { get; set; }
        public string ServerVerificationToken { get; set; }
    }
}
