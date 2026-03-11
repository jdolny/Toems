using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserGroupRight(ServiceContext ctx)
    {
        public DtoActionResult AddUserGroupRights(List<EntityUserGroupRight> listOfRights)
        {
            if (!listOfRights.Any()) return new DtoActionResult();
            var userGroupId = listOfRights.First().UserGroupId;
            ctx.Uow.UserGroupRightRepository.DeleteRange(x => x.UserGroupId == userGroupId);
            ctx.Uow.Save();

            foreach (var right in listOfRights)
                ctx.Uow.UserGroupRightRepository.Insert(right);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            return actionResult;
        }
    }
}