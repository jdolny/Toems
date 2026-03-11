using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserGroupMembership(ServiceContext ctx)
    {
        public DtoActionResult AddMembership(List<EntityUserGroupMembership> groupMemberships)
        {
            var actionResult = new DtoActionResult();
            if (!groupMemberships.Any()) return actionResult;
           
            foreach (var membership in groupMemberships)
            {
                if (
                    ctx.Uow.UserGroupMembershipRepository.Exists(
                        x => x.ToemsUserId == membership.ToemsUserId && x.UserGroupId == membership.UserGroupId))
                    continue;
                ctx.Uow.UserGroupMembershipRepository.Insert(membership);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;


         

            return actionResult;
        }

        public DtoActionResult AddMembership(EntityUserGroupMembership groupMembership)
        {
            var actionResult = new DtoActionResult();

                if (!ctx.Uow.UserGroupMembershipRepository.Exists(
                        x => x.ToemsUserId == groupMembership.ToemsUserId && x.UserGroupId == groupMembership.UserGroupId))
     
                ctx.Uow.UserGroupMembershipRepository.Insert(groupMembership);
            

            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;




            return actionResult;
        }



        public bool DeleteByIds(int userId, int userGroupId)
        {
            ctx.Uow.UserGroupMembershipRepository.DeleteRange(x => x.ToemsUserId == userId && x.UserGroupId == userGroupId);
            ctx.Uow.Save();
            return true;
        }

      

     

      
    }
}