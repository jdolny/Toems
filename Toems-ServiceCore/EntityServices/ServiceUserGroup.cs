using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserGroup(EntityContext ectx)
    {

        public DtoActionResult AddUserGroup(EntityToemsUserGroup userGroup)
        {
            var validationResult = ValidateUserGroup(userGroup, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.UserGroupRepository.Insert(userGroup);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = userGroup.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult DeleteUserGroup(int userGroupId)
        {
            var ug = GetUserGroup(userGroupId);
            if (ug == null) return new DtoActionResult {ErrorMessage = "User Group Not Found", Id = 0};

            var legacyGroupMembers = ectx.Uow.UserRepository.Get(x => x.UserGroupId == userGroupId);

            foreach (var groupMember in legacyGroupMembers)
            {
                groupMember.UserGroupId = -1;
                ectx.Uow.UserRepository.Update(groupMember,groupMember.Id);
            }

            ectx.Uow.UserGroupRepository.Delete(userGroupId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = ug.Id;
            return actionResult;
        }

     
        public List<int> GetManagedImageIds(int userGroupId)
        {
            return ectx.Uow.UserGroupImagesRepository.Get(x => x.UserGroupId == userGroupId).Select(x => x.ImageId).ToList();
        }

        public List<int> GetManagedGroupIds(int userGroupId)
        {
            return ectx.Uow.UserGroupComputerGroupsRepository.Get(x => x.UserGroupId == userGroupId).Select(x => x.GroupId).ToList();
        }


        public bool DeleteUserGroupRights(int userGroupId)
        {
            ectx.Uow.UserGroupRightRepository.DeleteRange(x => x.UserGroupId == userGroupId);
            ectx.Uow.Save();
            return true;
        }

        public List<EntityToemsUser> GetGroupMembers(int userGroupId, DtoSearchFilter filter)
        {
            return ectx.Uow.UserRepository.GetGroupMembers(userGroupId);
        }

        public List<EntityToemsUserGroup> GetLdapGroups()
        {
            return ectx.Uow.UserGroupRepository.Get(x => x.IsLdapGroup == 1);
        }

        public EntityToemsUserGroup GetUserGroup(int userGroupId)
        {
            return ectx.Uow.UserGroupRepository.GetById(userGroupId);
        }

     

        public List<EntityUserGroupRight> GetUserGroupRights(int userGroupId)
        {
            return ectx.Uow.UserGroupRightRepository.Get(x => x.UserGroupId == userGroupId);
        }

        public string MemberCount(int userGroupId)
        {
            return ectx.Uow.UserGroupMembershipRepository.Count(g => g.UserGroupId == userGroupId);
        }

        public List<EntityToemsUserGroup> SearchUserGroups(DtoSearchFilter filter)
        {
            return ectx.Uow.UserGroupRepository.Get(u => u.Name.Contains(filter.SearchText));
        }

        public bool ToggleGroupManagement(int userGroupId, int value)
        {
            var cdUserGroup = GetUserGroup(userGroupId);
           
            var result = UpdateUserGroup(cdUserGroup);
            foreach (var user in GetGroupMembers(userGroupId, new DtoSearchFilter()))
            {
                
                ectx.Uow.UserRepository.Update(user, user.Id);
            }
            ectx.Uow.Save();
            return result.Success;
        }

        public bool ToggleImageManagement(int userGroupId, int value)
        {
            var cdUserGroup = GetUserGroup(userGroupId);
           
            var result = UpdateUserGroup(cdUserGroup);

            foreach (var user in GetGroupMembers(userGroupId,new DtoSearchFilter()))
            {
               
                ectx.Uow.UserRepository.Update(user, user.Id);
            }
            ectx.Uow.Save();
            return result.Success;
        }

        public string TotalCount()
        {
            return ectx.Uow.UserGroupRepository.Count();
        }

        public DtoActionResult UpdateUserGroup(EntityToemsUserGroup userGroup)
        {
            var ug = GetUserGroup(userGroup.Id);
            if (ug == null) return new DtoActionResult {ErrorMessage = "User Group Not Found", Id = 0};
            var actionResult = new DtoActionResult();
            var validationResult = ValidateUserGroup(userGroup, false);
            if (validationResult.Success)
            {
                ectx.Uow.UserGroupRepository.Update(userGroup, userGroup.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = userGroup.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }
        public bool RemoveMembership(int userId, int groupId)
        {
            ectx.Uow.UserGroupMembershipRepository.DeleteRange(
                g => g.ToemsUserId == userId && g.UserGroupId == groupId);
            ectx.Uow.Save();
            return true;
        }

        private DtoValidationResult ValidateUserGroup(EntityToemsUserGroup userGroup, bool isNewUserGroup)
        {
            var validationResult = new DtoValidationResult {Success = true};

            if (isNewUserGroup)
            {
                if (ectx.Uow.UserGroupRepository.Exists(h => h.Name == userGroup.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "This User Group Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalUserGroup = ectx.Uow.UserGroupRepository.GetById(userGroup.Id);
                if (originalUserGroup.Name != userGroup.Name)
                {
                    if (ectx.Uow.UserGroupRepository.Exists(h => h.Name == userGroup.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "This User Group Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }
    }
}