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
    public class ServiceImageProfileFileCopy
    {
        private readonly UnitOfWork _uow;

        public ServiceImageProfileFileCopy()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddImageProfileFileCopy(EntityImageProfileFileCopy imageProfileFileCopy)
        {
            _uow.ImageProfileFileCopyRepository.Insert(imageProfileFileCopy);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileFileCopy.Id;
            return actionResult;
        }
    }
}
