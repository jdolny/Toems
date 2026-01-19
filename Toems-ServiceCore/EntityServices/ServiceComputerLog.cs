using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerLog(EntityContext ectx)
    {
        public DtoActionResult AddComputerLog(EntityComputerLog computerLog)
        {
            var actionResult = new DtoActionResult();
            computerLog.LogTime = DateTime.Now;
            ectx.Uow.ComputerLogRepository.Insert(computerLog);
            ectx.Uow.Save();
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

            ectx.Uow.ComputerLogRepository.Delete(computerLogId);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = computerLogId;

            return actionResult;
        }

        public EntityComputerLog GetComputerLog(int computerLogId)
        {
            return ectx.Uow.ComputerLogRepository.GetById(computerLogId);
        }

        public List<EntityComputerLog> SearchUnreg(int limit)
        {
            return
                ectx.Uow.ComputerLogRepository.Get(x => x.ComputerId <= -1, q => q.OrderByDescending(x => x.LogTime))
                    .Take(limit)
                    .ToList();
        }
    }
}
