using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserRight(ServiceContext ctx)
    {
        public DtoActionResult AddUserRights(List<EntityUserRight> listOfRights)
        {
            var userId = listOfRights.First().UserId;
            if (!listOfRights.Any()) return new DtoActionResult();
            ctx.Uow.UserRightRepository.DeleteRange(x => x.UserId == userId);
            ctx.Uow.Save();

            foreach (var right in listOfRights)
                ctx.Uow.UserRightRepository.Insert(right);

            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}