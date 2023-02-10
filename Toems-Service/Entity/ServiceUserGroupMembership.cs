using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceUserGroupMembership
    {
        private readonly UnitOfWork _uow;

        public ServiceUserGroupMembership()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddMembership(List<EntityUserGroupMembership> groupMemberships)
        {
            var actionResult = new DtoActionResult();
            if (!groupMemberships.Any()) return actionResult;
           
            foreach (var membership in groupMemberships)
            {
                if (
                    _uow.UserGroupMembershipRepository.Exists(
                        x => x.ToemsUserId == membership.ToemsUserId && x.UserGroupId == membership.UserGroupId))
                    continue;
                _uow.UserGroupMembershipRepository.Insert(membership);
            }

            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;


         

            return actionResult;
        }

        public DtoActionResult AddMembership(EntityUserGroupMembership groupMembership)
        {
            var actionResult = new DtoActionResult();

                if (!_uow.UserGroupMembershipRepository.Exists(
                        x => x.ToemsUserId == groupMembership.ToemsUserId && x.UserGroupId == groupMembership.UserGroupId))
     
                _uow.UserGroupMembershipRepository.Insert(groupMembership);
            

            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;




            return actionResult;
        }



        public bool DeleteByIds(int userId, int userGroupId)
        {
            _uow.UserGroupMembershipRepository.DeleteRange(x => x.ToemsUserId == userId && x.UserGroupId == userGroupId);
            _uow.Save();
            return true;
        }

      

     

      
    }
}