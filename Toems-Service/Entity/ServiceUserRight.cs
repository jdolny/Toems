using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceUserRight
    {
        private readonly UnitOfWork _uow;

        public ServiceUserRight()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddUserRights(List<EntityUserRight> listOfRights)
        {
            foreach (var right in listOfRights)
                _uow.UserRightRepository.Insert(right);

            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}