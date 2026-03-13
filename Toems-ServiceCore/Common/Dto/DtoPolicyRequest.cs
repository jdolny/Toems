using System.Collections.Generic;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_Common.Dto
{
    public class DtoPolicyRequest
    {
        public EnumPolicy.Trigger Trigger { get; set; }
        public DtoClientIdentity ClientIdentity;
        public List<EntityUserLogin> UserLogins { get; set; }
        public List<DtoAppMonitor> AppMonitors { get; set; }
        public string ClientVersion { get; set; }
        public string PushURL { get; set; }
        public string CurrentComServer { get; set; }
    }
}
