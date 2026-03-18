using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Data;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserGroup(ServiceContext ctx)
    {

        public DtoActionResult AddUserGroup(EntityToemsUserGroup userGroup)
        {
            var validationResult = ValidateUserGroup(userGroup, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ctx.Uow.UserGroupRepository.Insert(userGroup);
                ctx.Uow.Save();
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

            var legacyGroupMembers = ctx.Uow.UserRepository.Get(x => x.UserGroupId == userGroupId);

            foreach (var groupMember in legacyGroupMembers)
            {
                groupMember.UserGroupId = -1;
                ctx.Uow.UserRepository.Update(groupMember,groupMember.Id);
            }

            ctx.Uow.UserGroupRepository.Delete(userGroupId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = ug.Id;
            return actionResult;
        }

     
        public List<int> GetManagedImageIds(int userGroupId)
        {
            return ctx.Uow.UserGroupImagesRepository.Get(x => x.UserGroupId == userGroupId).Select(x => x.ImageId).ToList();
        }

        public List<int> GetManagedGroupIds(int userGroupId)
        {
            return ctx.Uow.UserGroupComputerGroupsRepository.Get(x => x.UserGroupId == userGroupId).Select(x => x.GroupId).ToList();
        }


        public bool DeleteUserGroupRights(int userGroupId)
        {
            ctx.Uow.UserGroupRightRepository.DeleteRange(x => x.UserGroupId == userGroupId);
            ctx.Uow.Save();
            return true;
        }

        public List<AppUser> GetGroupMembers(int userGroupId, DtoSearchFilter filter)
        {
            return ctx.Uow.UserRepository.GetGroupMembers(userGroupId);
        }

        public List<EntityToemsUserGroup> GetLdapGroups()
        {
            return ctx.Uow.UserGroupRepository.Get(x => x.IsLdapGroup == 1);
        }

        public EntityToemsUserGroup GetUserGroup(int userGroupId)
        {
            return ctx.Uow.UserGroupRepository.GetById(userGroupId);
        }

     

        public List<EntityUserGroupRight> GetUserGroupRights(int userGroupId)
        {
            return ctx.Uow.UserGroupRightRepository.Get(x => x.UserGroupId == userGroupId);
        }

        public string MemberCount(int userGroupId)
        {
            return ctx.Uow.UserGroupMembershipRepository.Count(g => g.UserGroupId == userGroupId);
        }

        public List<EntityToemsUserGroup> SearchUserGroups(DtoSearchFilter filter)
        {
            return ctx.Uow.UserGroupRepository.Get(u => u.Name.Contains(filter.SearchText));
        }

        public bool ToggleGroupManagement(int userGroupId, int value)
        {
            var cdUserGroup = GetUserGroup(userGroupId);
           
            var result = UpdateUserGroup(cdUserGroup);
            foreach (var user in GetGroupMembers(userGroupId, new DtoSearchFilter()))
            {
                
                ctx.Uow.UserRepository.Update(user, user.Id);
            }
            ctx.Uow.Save();
            return result.Success;
        }

        public bool ToggleImageManagement(int userGroupId, int value)
        {
            var cdUserGroup = GetUserGroup(userGroupId);
           
            var result = UpdateUserGroup(cdUserGroup);

            foreach (var user in GetGroupMembers(userGroupId,new DtoSearchFilter()))
            {
               
                ctx.Uow.UserRepository.Update(user, user.Id);
            }
            ctx.Uow.Save();
            return result.Success;
        }

        public string TotalCount()
        {
            return ctx.Uow.UserGroupRepository.Count();
        }

        public DtoActionResult UpdateUserGroup(EntityToemsUserGroup userGroup)
        {
            var ug = GetUserGroup(userGroup.Id);
            if (ug == null) return new DtoActionResult {ErrorMessage = "User Group Not Found", Id = 0};
            var actionResult = new DtoActionResult();
            var validationResult = ValidateUserGroup(userGroup, false);
            if (validationResult.Success)
            {
                ctx.Uow.UserGroupRepository.Update(userGroup, userGroup.Id);
                ctx.Uow.Save();
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
            ctx.Uow.UserGroupMembershipRepository.DeleteRange(
                g => g.ToemsUserId == userId && g.UserGroupId == groupId);
            ctx.Uow.Save();
            return true;
        }

        private DtoValidationResult ValidateUserGroup(EntityToemsUserGroup userGroup, bool isNewUserGroup)
        {
            var validationResult = new DtoValidationResult {Success = true};

            if (isNewUserGroup)
            {
                if (ctx.Uow.UserGroupRepository.Exists(h => h.Name == userGroup.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "This User Group Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalUserGroup = ctx.Uow.UserGroupRepository.GetById(userGroup.Id);
                if (originalUserGroup.Name != userGroup.Name)
                {
                    if (ctx.Uow.UserGroupRepository.Exists(h => h.Name == userGroup.Name))
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