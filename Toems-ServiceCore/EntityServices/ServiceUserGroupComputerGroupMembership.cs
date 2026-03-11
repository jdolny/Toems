using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserGroupComputerGroupMembership(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityUserGroupComputerGroups> groups, int userGroupId)
        {
            var actionResult = new DtoActionResult();
         
            var pToRemove = ctx.Uow.UserGroupComputerGroupsRepository.Get(x => x.UserGroupId == userGroupId);
            foreach (var group in groups)
            {
                var existing = ctx.Uow.UserGroupComputerGroupsRepository.GetFirstOrDefault(x => x.UserGroupId == userGroupId && x.GroupId == group.GroupId);
                    

                if (existing == null)
                {
                    ctx.Uow.UserGroupComputerGroupsRepository.Insert(group);
                }
                else
                {
                    pToRemove.Remove(existing);
                }
                
            }

            //anything left in pToRemove is no longer part of the image management
            foreach (var p in pToRemove)
            {
                ctx.Uow.UserGroupComputerGroupsRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}