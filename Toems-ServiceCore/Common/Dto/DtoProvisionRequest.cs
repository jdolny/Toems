using System.Collections.Generic;

namespace Toems_Common.Dto
{
    public class DtoProvisionRequest
    {
        public DtoProvisionRequest()
        {
            Macs = new List<string>();
        }
        public string Name { get; set; }
        public string AdGuid { get; set; }
        public string SymmKey { get; set; }
        public string InstallationId { get; set; }
        public string Processor { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public int Memory { get; set; }
        public List<string> Macs { get; set; }
    }
}
