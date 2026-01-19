using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveSocket(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(string computerGuid, string connectionId, string comServer)
        {
            var computer = ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid.Equals(computerGuid));
            if(computer == null)
                return new DtoActionResult { ErrorMessage = "Computer Not Found", Id = 0 };

            var activeSocket = new EntityActiveSocket();
            activeSocket.ComputerId = computer.Id;
            activeSocket.ConnectionId = connectionId;
            activeSocket.ComServer = comServer;

            var actionResult = new DtoActionResult();
            var existing = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (existing == null)
            {
                ectx.Uow.ActiveSocketRepository.Insert(activeSocket);
            }
            else
            {
                activeSocket.Id = existing.Id;
                ectx.Uow.ActiveSocketRepository.Update(activeSocket, activeSocket.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = activeSocket.Id;

            return actionResult;
        }

        public bool Delete(string connectionId)
        {
            ectx.Uow.ActiveSocketRepository.DeleteRange(x => x.ConnectionId.Equals(connectionId));
            ectx.Uow.Save();
            return true;
        }
    }
}