using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceGroupMembership
    {
        private readonly UnitOfWork _uow;

        public ServiceGroupMembership()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddMembership(List<EntityGroupMembership> groupMemberships)
        {
            var actionResult = new DtoActionResult();
            foreach (var membership in groupMemberships)
            {
                if (
                    _uow.GroupMembershipRepository.Exists(
                        x => x.ComputerId == membership.ComputerId && x.GroupId == membership.GroupId))
                    continue;
                _uow.GroupMembershipRepository.Insert(membership);
            }

            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;

            return actionResult;
        }

        public bool DeleteByIds(int computerId, int groupId)
        {
            _uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == computerId && x.GroupId == groupId);
            _uow.Save();
            return true;
        }

      

     

      
    }
}