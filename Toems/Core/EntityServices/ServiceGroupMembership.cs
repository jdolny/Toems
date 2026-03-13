using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceGroupMembership(ServiceContext ctx)
    {
        public DtoActionResult AddMembership(List<EntityGroupMembership> groupMemberships)
        {
            var actionResult = new DtoActionResult();
            foreach (var membership in groupMemberships)
            {
                if (
                    ctx.Uow.GroupMembershipRepository.Exists(
                        x => x.ComputerId == membership.ComputerId && x.GroupId == membership.GroupId))
                    continue;
                ctx.Uow.GroupMembershipRepository.Insert(membership);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;

            return actionResult;
        }

        public bool DeleteByIds(int computerId, int groupId)
        {
            ctx.Uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == computerId && x.GroupId == groupId);
            ctx.Uow.Save();
            return true;
        }

      

     

      
    }
}