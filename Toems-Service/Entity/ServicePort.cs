using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServicePort
    {
        private readonly UnitOfWork _uow;

        public ServicePort()
        {
            _uow = new UnitOfWork();
        }

        public bool AddPort(EntityMulticastPort port)
        {
            _uow.MulticastPortRepository.Insert(port);
            _uow.Save();
            return true;
        }

        public int GetNextPort(int? comServerId)
        {
            var comServer = _uow.ClientComServerRepository.GetById(comServerId);
            var nextPort = new EntityMulticastPort();
            nextPort.ComServerId = comServer.Id;
            var lastPort = _uow.MulticastPortRepository.Get(x => x.ComServerId == comServerId).OrderByDescending(x => x.Id).FirstOrDefault();

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
