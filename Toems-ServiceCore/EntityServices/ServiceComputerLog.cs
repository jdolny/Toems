using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerLog(ServiceContext ctx)
    {
        public DtoActionResult AddComputerLog(EntityComputerLog computerLog)
        {
            var actionResult = new DtoActionResult();
            computerLog.LogTime = DateTime.Now;
            ctx.Uow.ComputerLogRepository.Insert(computerLog);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = computerLog.Id;

            return actionResult;
        }

        public DtoActionResult DeleteComputerLog(int computerLogId)
        {
            var computerLog = GetComputerLog(computerLogId);
            if (computerLog == null)
                return new DtoActionResult { ErrorMessage = "Computer Log Not Found", Id = 0 };

            var actionResult = new DtoActionResult();

            ctx.Uow.ComputerLogRepository.Delete(computerLogId);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = computerLogId;

            return actionResult;
        }

        public EntityComputerLog GetComputerLog(int computerLogId)
        {
            return ctx.Uow.ComputerLogRepository.GetById(computerLogId);
        }

        public List<EntityComputerLog> SearchUnreg(int limit)
        {
            return
                ctx.Uow.ComputerLogRepository.Get(x => x.ComputerId <= -1, q => q.OrderByDescending(x => x.LogTime))
                    .Take(limit)
                    .ToList();
        }
    }
}
