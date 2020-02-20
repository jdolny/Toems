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
    public class ServiceImageProfileScript
    {
        private readonly UnitOfWork _uow;

        public ServiceImageProfileScript()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddImageProfileScript(EntityImageProfileScript imageProfileScript)
        {
            _uow.ImageProfileScriptRepository.Insert(imageProfileScript);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileScript.Id;
            return actionResult;
        }
    }
}
