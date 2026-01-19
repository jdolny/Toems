using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Entity
{
    public class ServiceUserGroupMembership(EntityContext ectx)
    {
        public DtoActionResult AddMembership(List<EntityUserGroupMembership> groupMemberships)
        {
            var actionResult = new DtoActionResult();
            if (!groupMemberships.Any()) return actionResult;
           
            foreach (var membership in groupMemberships)
            {
                if (
                    ectx.Uow.UserGroupMembershipRepository.Exists(
                        x => x.ToemsUserId == membership.ToemsUserId && x.UserGroupId == membership.UserGroupId))
                    continue;
                ectx.Uow.UserGroupMembershipRepository.Insert(membership);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;


         

            return actionResult;
        }

        public DtoActionResult AddMembership(EntityUserGroupMembership groupMembership)
        {
            var actionResult = new DtoActionResult();

                if (!ectx.Uow.UserGroupMembershipRepository.Exists(
                        x => x.ToemsUserId == groupMembership.ToemsUserId && x.UserGroupId == groupMembership.UserGroupId))
     
                ectx.Uow.UserGroupMembershipRepository.Insert(groupMembership);
            

            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;




            return actionResult;
        }



        public bool DeleteByIds(int userId, int userGroupId)
        {
            ectx.Uow.UserGroupMembershipRepository.DeleteRange(x => x.ToemsUserId == userId && x.UserGroupId == userGroupId);
            ectx.Uow.Save();
            return true;
        }

      

     

      
    }
}