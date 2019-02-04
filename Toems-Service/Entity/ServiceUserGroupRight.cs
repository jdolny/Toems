using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceUserGroupRight
    {
        private readonly UnitOfWork _uow;

        public ServiceUserGroupRight()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddUserGroupRights(List<EntityUserGroupRight> listOfRights)
        {
            foreach (var right in listOfRights)
                _uow.UserGroupRightRepository.Insert(right);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            return actionResult;
        }
    }
}