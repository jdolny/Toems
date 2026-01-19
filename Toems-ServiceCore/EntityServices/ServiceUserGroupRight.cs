using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserGroupRight(EntityContext ectx)
    {
        public DtoActionResult AddUserGroupRights(List<EntityUserGroupRight> listOfRights)
        {
            if (!listOfRights.Any()) return new DtoActionResult();
            var userGroupId = listOfRights.First().UserGroupId;
            ectx.Uow.UserGroupRightRepository.DeleteRange(x => x.UserGroupId == userGroupId);
            ectx.Uow.Save();

            foreach (var right in listOfRights)
                ectx.Uow.UserGroupRightRepository.Insert(right);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            return actionResult;
        }
    }
}