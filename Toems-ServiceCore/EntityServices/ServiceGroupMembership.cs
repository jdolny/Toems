using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceGroupMembership(EntityContext ectx)
    {
        public DtoActionResult AddMembership(List<EntityGroupMembership> groupMemberships)
        {
            var actionResult = new DtoActionResult();
            foreach (var membership in groupMemberships)
            {
                if (
                    ectx.Uow.GroupMembershipRepository.Exists(
                        x => x.ComputerId == membership.ComputerId && x.GroupId == membership.GroupId))
                    continue;
                ectx.Uow.GroupMembershipRepository.Insert(membership);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;

            return actionResult;
        }

        public bool DeleteByIds(int computerId, int groupId)
        {
            ectx.Uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == computerId && x.GroupId == groupId);
            ectx.Uow.Save();
            return true;
        }

      

     

      
    }
}