using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveSocket(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(string computerGuid, string connectionId, string comServer)
        {
            var computer = ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid.Equals(computerGuid));
            if(computer == null)
                return new DtoActionResult { ErrorMessage = "Computer Not Found", Id = 0 };

            var activeSocket = new EntityActiveSocket();
            activeSocket.ComputerId = computer.Id;
            activeSocket.ConnectionId = connectionId;
            activeSocket.ComServer = comServer;

            var actionResult = new DtoActionResult();
            var existing = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (existing == null)
            {
                ctx.Uow.ActiveSocketRepository.Insert(activeSocket);
            }
            else
            {
                activeSocket.Id = existing.Id;
                ctx.Uow.ActiveSocketRepository.Update(activeSocket, activeSocket.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = activeSocket.Id;

            return actionResult;
        }

        public bool Delete(string connectionId)
        {
            ctx.Uow.ActiveSocketRepository.DeleteRange(x => x.ConnectionId.Equals(connectionId));
            ctx.Uow.Save();
            return true;
        }
    }
}