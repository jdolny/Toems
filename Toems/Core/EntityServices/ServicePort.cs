using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePort(ServiceContext ctx)
    {
        public bool AddPort(EntityMulticastPort port)
        {
            ctx.Uow.MulticastPortRepository.Insert(port);
            ctx.Uow.Save();
            return true;
        }

        public int GetNextPort(int? comServerId)
        {
            var comServer = ctx.Uow.ClientComServerRepository.GetById(comServerId);
            var nextPort = new EntityMulticastPort();
            nextPort.ComServerId = comServer.Id;
            var lastPort = ctx.Uow.MulticastPortRepository.Get(x => x.ComServerId == comServerId).OrderByDescending(x => x.Id).FirstOrDefault();

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
