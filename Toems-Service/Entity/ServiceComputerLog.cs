using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputerLog
    {
        private readonly UnitOfWork _uow;

        public ServiceComputerLog()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddComputerLog(EntityComputerLog computerLog)
        {
            var actionResult = new DtoActionResult();
            computerLog.LogTime = DateTime.Now;
            _uow.ComputerLogRepository.Insert(computerLog);
            _uow.Save();
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

            _uow.ComputerLogRepository.Delete(computerLogId);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = computerLogId;

            return actionResult;
        }

        public EntityComputerLog GetComputerLog(int computerLogId)
        {
            return _uow.ComputerLogRepository.GetById(computerLogId);
        }

        public List<EntityComputerLog> SearchUnreg(int limit)
        {
            return
                _uow.ComputerLogRepository.Get(x => x.ComputerId <= -1, q => q.OrderByDescending(x => x.LogTime))
                    .Take(limit)
                    .ToList();
        }
    }
}
