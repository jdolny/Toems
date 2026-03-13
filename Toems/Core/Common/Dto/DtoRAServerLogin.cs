using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoRAServerLogin
    {
        public string authCookie { get; set; }
        public string currentNode { get; set; }
        public string domainurl { get; set; }
        public string domain { get; set; }
        public string debuglevel { get; set; }
        public string serverDnsName { get; set; }
        public string serverRedirPort { get; set; }
        public string serverPublicPort { get; set; }
        public string noServerBackup { get; set; }
        public string passRequirements { get; set; }
        public string webcerthash { get; set; }
        public string xid { get; set; }
        public string xidsig { get; set; }
    }
}
