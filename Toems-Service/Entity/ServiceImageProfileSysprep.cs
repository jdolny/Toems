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
    public class ServiceImageProfileSysprep
    {
        private readonly UnitOfWork _uow;

        public ServiceImageProfileSysprep()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddImageProfileSysprep(EntityImageProfileSysprepTag imageProfileSysprep)
        {
            _uow.ImageProfileSysprepRepository.Insert(imageProfileSysprep);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileSysprep.Id;
            return actionResult;
        }
    }
}
