using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserRight(EntityContext ectx)
    {
        public DtoActionResult AddUserRights(List<EntityUserRight> listOfRights)
        {
            var userId = listOfRights.First().UserId;
            if (!listOfRights.Any()) return new DtoActionResult();
            ectx.Uow.UserRightRepository.DeleteRange(x => x.UserId == userId);
            ectx.Uow.Save();

            foreach (var right in listOfRights)
                ectx.Uow.UserRightRepository.Insert(right);

            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}