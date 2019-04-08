using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceActiveSocket
    {
        private readonly UnitOfWork _uow;

        public ServiceActiveSocket()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(string computerGuid, string connectionId, string comServer)
        {
            var computer = _uow.ComputerRepository.GetFirstOrDefault(x => x.Guid.Equals(computerGuid));
            if(computer == null)
                return new DtoActionResult { ErrorMessage = "Computer Not Found", Id = 0 };

            var activeSocket = new EntityActiveSocket();
            activeSocket.ComputerId = computer.Id;
            activeSocket.ConnectionId = connectionId;
            activeSocket.ComServer = comServer;

            var actionResult = new DtoActionResult();
            var existing = _uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (existing == null)
            {
                _uow.ActiveSocketRepository.Insert(activeSocket);
            }
            else
            {
                activeSocket.Id = existing.Id;
                _uow.ActiveSocketRepository.Update(activeSocket, activeSocket.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = activeSocket.Id;

            return actionResult;
        }

        public bool Delete(string connectionId)
        {
            _uow.ActiveSocketRepository.DeleteRange(x => x.ConnectionId.Equals(connectionId));
            _uow.Save();
            return true;
        }
    }
}