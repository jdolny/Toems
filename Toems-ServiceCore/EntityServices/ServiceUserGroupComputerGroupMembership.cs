using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Entity
{
    public class ServiceUserGroupComputerGroupMembership(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityUserGroupComputerGroups> groups, int userGroupId)
        {
            var actionResult = new DtoActionResult();
         
            var pToRemove = ectx.Uow.UserGroupComputerGroupsRepository.Get(x => x.UserGroupId == userGroupId);
            foreach (var group in groups)
            {
                var existing = ectx.Uow.UserGroupComputerGroupsRepository.GetFirstOrDefault(x => x.UserGroupId == userGroupId && x.GroupId == group.GroupId);
                    

                if (existing == null)
                {
                    ectx.Uow.UserGroupComputerGroupsRepository.Insert(group);
                }
                else
                {
                    pToRemove.Remove(existing);
                }
                
            }

            //anything left in pToRemove is no longer part of the image management
            foreach (var p in pToRemove)
            {
                ectx.Uow.UserGroupComputerGroupsRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}