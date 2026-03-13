using System.Collections.Generic;
using Toems_Common.Dto.client;
using Toems_Common.Enum;

namespace Toems_Common.Dto
{
    public class DtoProvisionResponse
    {
        public DtoProvisionResponse()
        {
            ComServers = new List<DtoClientComServers>();
        }
        public EnumProvisionStatus.Status ProvisionStatus { get; set; }
        public string Certificate { get; set; }
        public string ComputerIdentifier { get; set; }
        public string Message { get; set; }
        public List<DtoClientComServers> ComServers { get; set; } 
    }
}
