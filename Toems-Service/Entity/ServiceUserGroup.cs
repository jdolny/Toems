using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceUserGroup
    {
        private readonly UnitOfWork _uow;
        private readonly ServiceUser _userServices;

        public ServiceUserGroup()
        {
            _uow = new UnitOfWork();
            _userServices = new ServiceUser();
        }

        public bool AddNewGroupMember(int userGroupId, int userId)
        {
            var user = new ServiceUser().GetUser(userId);
            var userGroup = GetUserGroup(userGroupId);
            user.Membership = userGroup.Membership;
            user.UserGroupId = userGroup.Id;
          
            new ServiceUser().UpdateUser(user);

            var rights = GetUserGroupRights(userGroup.Id);
         

            var userRights =
                rights.Select(right => new EntityUserRight {UserId = user.Id, Right = right.Right}).ToList();
            _userServices.DeleteUserRights(user.Id);
            new ServiceUserRight().AddUserRights(userRights);

        

          

            return true;
        }

        public DtoActionResult AddUserGroup(EntityToemsUserGroup userGroup)
        {
            var validationResult = ValidateUserGroup(userGroup, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.UserGroupRepository.Insert(userGroup);
                _uow.Save();
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

            var groupMembers = GetGroupMembers(userGroupId, new DtoSearchFilter());
            foreach (var groupMember in groupMembers)
            {
                groupMember.UserGroupId = -1;
                _uow.UserRepository.Update(groupMember,groupMember.Id);
            }

            _uow.UserGroupRepository.Delete(userGroupId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = ug.Id;
            return actionResult;
        }

     

        

        public bool DeleteUserGroupRights(int userGroupId)
        {
            _uow.UserGroupRightRepository.DeleteRange(x => x.UserGroupId == userGroupId);
            _uow.Save();
            return true;
        }

        public List<EntityToemsUser> GetGroupMembers(int userGroupId, DtoSearchFilter filter)
        {
            return _uow.UserRepository.Get(x => x.UserGroupId == userGroupId && x.Name.Contains(filter.SearchText),
                q => q.OrderBy(p => p.Name));
        }

        public List<EntityToemsUserGroup> GetLdapGroups()
        {
            return _uow.UserGroupRepository.Get(x => x.IsLdapGroup == 1);
        }

        public EntityToemsUserGroup GetUserGroup(int userGroupId)
        {
            return _uow.UserGroupRepository.GetById(userGroupId);
        }

     

        public List<EntityUserGroupRight> GetUserGroupRights(int userGroupId)
        {
            return _uow.UserGroupRightRepository.Get(x => x.UserGroupId == userGroupId);
        }

        public string MemberCount(int userGroupId)
        {
            return _uow.UserRepository.Count(x => x.UserGroupId == userGroupId);
        }

        public List<EntityToemsUserGroup> SearchUserGroups(DtoSearchFilter filter)
        {
            return _uow.UserGroupRepository.Get(u => u.Name.Contains(filter.SearchText));
        }

        public bool ToggleGroupManagement(int userGroupId, int value)
        {
            var cdUserGroup = GetUserGroup(userGroupId);
           
            var result = UpdateUserGroup(cdUserGroup);
            foreach (var user in GetGroupMembers(userGroupId, new DtoSearchFilter()))
            {
                
                _uow.UserRepository.Update(user, user.Id);
            }
            _uow.Save();
            return result.Success;
        }

        public bool ToggleImageManagement(int userGroupId, int value)
        {
            var cdUserGroup = GetUserGroup(userGroupId);
           
            var result = UpdateUserGroup(cdUserGroup);

            foreach (var user in GetGroupMembers(userGroupId,new DtoSearchFilter()))
            {
               
                _uow.UserRepository.Update(user, user.Id);
            }
            _uow.Save();
            return result.Success;
        }

        public string TotalCount()
        {
            return _uow.UserGroupRepository.Count();
        }

        public bool UpdateAllGroupMembersAcls(int userGroupId)
        {
            var rights = GetUserGroupRights(userGroupId);

            foreach (var user in GetGroupMembers(userGroupId,new DtoSearchFilter()))
            {
                var userRights =
                    rights.Select(right => new EntityUserRight {UserId = user.Id, Right = right.Right}).ToList();
                _userServices.DeleteUserRights(user.Id);
                new ServiceUserRight().AddUserRights(userRights);
            }

            return true;
        }

    

     

        public DtoActionResult UpdateUserGroup(EntityToemsUserGroup userGroup)
        {
            var ug = GetUserGroup(userGroup.Id);
            if (ug == null) return new DtoActionResult {ErrorMessage = "User Group Not Found", Id = 0};
            var actionResult = new DtoActionResult();
            var validationResult = ValidateUserGroup(userGroup, false);
            if (validationResult.Success)
            {
                _uow.UserGroupRepository.Update(userGroup, userGroup.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = userGroup.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateUserGroup(EntityToemsUserGroup userGroup, bool isNewUserGroup)
        {
            var validationResult = new DtoValidationResult {Success = true};

            if (isNewUserGroup)
            {
                if (_uow.UserGroupRepository.Exists(h => h.Name == userGroup.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "This User Group Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalUserGroup = _uow.UserGroupRepository.GetById(userGroup.Id);
                if (originalUserGroup.Name != userGroup.Name)
                {
                    if (_uow.UserGroupRepository.Exists(h => h.Name == userGroup.Name))
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