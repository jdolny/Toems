using System.Collections.Generic;

namespace Toems_Common.Dto
{
    public class DtoTriggerResponse
    {
        public DtoTriggerResponse()
        {
            Policies = new List<DtoClientPolicy>();
        }
        public int CheckinTime { get; set; }
        public int ShutdownDelay { get; set; }
        public bool UserLoginsSubmitted { get; set; }
        public bool AppMonitorSubmitted { get; set; }
        public List<DtoClientPolicy> Policies { get; set; }

    }
}
