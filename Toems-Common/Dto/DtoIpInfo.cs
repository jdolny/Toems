using System.Net;

namespace Toems_Common.Dto
{
    public class DtoIpInfo
    {
        public IPAddress IpAddress { get; set; }
        public IPAddress Netmask { get; set; }
        public IPAddress Gateway { get; set; }
        public IPAddress Broadcast { get; set; }
    }
}
