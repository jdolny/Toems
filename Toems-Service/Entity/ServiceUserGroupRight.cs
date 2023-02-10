using System.Collections.Generic;
using System.Linq;
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
            if (!listOfRights.Any()) return new DtoActionResult();
            var userGroupId = listOfRights.First().UserGroupId;
            _uow.UserGroupRightRepository.DeleteRange(x => x.UserGroupId == userGroupId);
            _uow.Save();

            foreach (var right in listOfRights)
                _uow.UserGroupRightRepository.Insert(right);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            return actionResult;
        }
    }
}