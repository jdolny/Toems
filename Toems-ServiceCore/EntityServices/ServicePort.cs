using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePort(EntityContext ectx)
    {
        public bool AddPort(EntityMulticastPort port)
        {
            ectx.Uow.MulticastPortRepository.Insert(port);
            ectx.Uow.Save();
            return true;
        }

        public int GetNextPort(int? comServerId)
        {
            var comServer = ectx.Uow.ClientComServerRepository.GetById(comServerId);
            var nextPort = new EntityMulticastPort();
            nextPort.ComServerId = comServer.Id;
            var lastPort = ectx.Uow.MulticastPortRepository.Get(x => x.ComServerId == comServerId).OrderByDescending(x => x.Id).FirstOrDefault();

            if (lastPort == null)
                nextPort.Number = comServer.MulticastStartPort;
            else if (lastPort.Number >= comServer.MulticastEndPort)
                nextPort.Number = comServer.MulticastStartPort;
            else
                nextPort.Number = lastPort.Number + 2;

            AddPort(nextPort);

            return nextPort.Number;
        }
    }
}
