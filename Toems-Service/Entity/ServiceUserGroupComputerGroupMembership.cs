using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceUserGroupComputerGroupMembership
    {
        private readonly UnitOfWork _uow;

        public ServiceUserGroupComputerGroupMembership()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityUserGroupComputerGroups> groups, int userGroupId)
        {
            var actionResult = new DtoActionResult();
         
            var pToRemove = _uow.UserGroupComputerGroupsRepository.Get(x => x.UserGroupId == userGroupId);
            foreach (var group in groups)
            {
                var existing = _uow.UserGroupComputerGroupsRepository.GetFirstOrDefault(x => x.UserGroupId == userGroupId && x.GroupId == group.GroupId);
                    

                if (existing == null)
                {
                    _uow.UserGroupComputerGroupsRepository.Insert(group);
                }
                else
                {
                    pToRemove.Remove(existing);
                }
                
            }

            //anything left in pToRemove is no longer part of the image management
            foreach (var p in pToRemove)
            {
                _uow.UserGroupComputerGroupsRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}