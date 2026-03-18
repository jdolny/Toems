using log4net;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Data;
using Toems_ServiceCore.Infrastructure;
using TwoFactorAuthNet;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUser(ServiceContext ctx)
    {
        public DtoActionResult AddUser(AppUser user)
        {
            user.ImagingToken = Guid.NewGuid().ToString("N").ToUpper() + Guid.NewGuid().ToString("N").ToUpper();
            var validationResult = ValidateUser(user, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ctx.Uow.UserRepository.Insert(user);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = user.UserId;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult DeleteUser(int userId)
        {
            var u = GetUser(userId);
            if (u == null) return new DtoActionResult { ErrorMessage = "User Not Found", Id = 0 };
            if (GetAdminCount() == 1 && u.Membership == "Administrator")
                return new DtoActionResult() { ErrorMessage = "There Must Be At Least 1 Administrator" };
            if (u.Membership.Equals("Administrator"))
                return new DtoActionResult()
                    {ErrorMessage = "Administrators Must Be Changed To The User Role Before They Can Be Removed"};
            ctx.Uow.UserRepository.Delete(userId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.UserId;
            return actionResult;
        }

        public int GetAdminCount()
        {
            return Convert.ToInt32(ctx.Uow.UserRepository.Count(u => u.Membership == "Administrator"));
        }

        public AppUser GetUser(int userId)
        {
            return ctx.Uow.UserRepository.GetById(userId);
        }

        public AppUser GetUser(string userName)
        {
            return ctx.Uow.UserRepository.GetFirstOrDefault(u => u.UserName == userName);
        }

        public AppUser GetUserForLogin(string userName)
        {
            var user = ctx.Uow.UserRepository.GetFirstOrDefault(u => u.UserName == userName);
            if (user == null) return null;
            return user;
        }

        public string GetUserName(int userId)
        {
            return ctx.Uow.UserRepository.GetUserName(userId);
        }

        public List<EntityUserRight> GetEffectiveUserRights(int userId)
        {
            var userGroups = ctx.Uow.UserGroupMembershipRepository.Get(x => x.ToemsUserId == userId).ToList();

            List<EntityUserGroupRight> allUserGroupRights = new List<EntityUserGroupRight>();
            foreach (var usergroup in userGroups)
            {
                allUserGroupRights.AddRange(ctx.Uow.UserGroupRightRepository.Get(x => x.UserGroupId == usergroup.UserGroupId));
            }

            var collectiveUserRights = allUserGroupRights.Select(right => new EntityUserRight { UserId = userId, Right = right.Right }).ToList();

            var userRights = ctx.Uow.UserRightRepository.Get(x => x.UserId == userId);
            if (userRights.Any())
            {
                collectiveUserRights.AddRange(userRights);
            }

            return collectiveUserRights;
        }

        public List<EntityAuditLog> GetUserAuditLogs(int userId, DtoSearchFilter filter)
        {
            if (filter.SearchText != "Select Filter")
                return
                ctx.Uow.AuditLogRepository.Get(x => x.UserId == userId && x.AuditType.ToString() == filter.SearchText).OrderByDescending(x => x.Id).Take(filter.Limit).ToList();
            else
            {
                return
                    ctx.Uow.AuditLogRepository.Get(x => x.UserId == userId).OrderByDescending(x => x.Id).Take(filter.Limit).ToList();
            }
        }

        public AppUser GetUserFromToken(string imagingToken)
        {
            return ctx.Uow.UserRepository.Get(x => x.ImagingToken.Equals(imagingToken)).FirstOrDefault();
        }
      

        public DtoApiObjectResponse GetUserForLogin(int userId)
        {
            var result = new DtoApiObjectResponse();
            var user = ctx.Uow.UserRepository.GetById(userId);
            if (user != null)
            {
                result.Success = true;
                result.Id = user.UserId;
                result.ObjectJson = JsonConvert.SerializeObject(user);
            }

            return result;
        }

        public AppUser GetUserWithPass(int userId)
        {
            return ctx.Uow.UserRepository.GetById(userId);

        }


        public List<DtoPinnedPolicy> GetPinnedPolicyCounts(int userId)
        {
            return ctx.Uow.UserRepository.GetUserPinnedPolicyCounts(userId);
        }

        public List<DtoPinnedGroup> GetPinnedGroupCounts(int userId)
        {

            return ctx.Uow.UserRepository.GetUserPinnedGroupCounts(userId);
        }

       
        public List<EntityUserRight> GetUserRights(int userId)
        {
            return ctx.Uow.UserRightRepository.Get(x => x.UserId == userId);
        }

        public bool UpdateUsersImagesList(EntityToemsUsersImages image)
        {
            ctx.Uow.ToemsUsersImagesRepository.Insert(image);
            ctx.Uow.Save();
            return true;
        }

        public bool UpdateUsersGroupsList(EntityToemsUsersGroups group)
        {
            ctx.Uow.ToemsUsersGroupsRepository.Insert(group);
            ctx.Uow.Save();
            return true;
        }



        public DtoUserImageManagement GetAllowedImages(int userId)
        {
            var dtoUserImageManagement = new DtoUserImageManagement();

            if (IsAdmin(userId)) return dtoUserImageManagement;


            foreach (var g in GetUsersGroups(userId))
            {
                if (g.EnableImageAcls)
                {
                    dtoUserImageManagement.ImageManagementEnforced = true;
                    dtoUserImageManagement.AllowedImageIds.AddRange(ctx.UserGroup.GetManagedImageIds(g.Id));
                    dtoUserImageManagement.AllowedImageIds.AddRange(ctx.Uow.ToemsUsersImagesRepository.Get(x => x.UserId == userId).Select(x => x.ImageId).ToList());
                }
            }

            dtoUserImageManagement.AllowedImageIds = dtoUserImageManagement.AllowedImageIds.Distinct().ToList();

            return dtoUserImageManagement;

        }

        public DtoUserGroupManagement GetAllowedGroups(int userId)
        {
            var dtoUserGroupManagement = new DtoUserGroupManagement();

            if (IsAdmin(userId)) return dtoUserGroupManagement;


            foreach (var g in GetUsersGroups(userId))
            {
                if (g.EnableComputerGroupAcls)
                {
                    dtoUserGroupManagement.GroupManagementEnforced = true;
                    dtoUserGroupManagement.AllowedGroupIds.AddRange(ctx.UserGroup.GetManagedGroupIds(g.Id));
                    dtoUserGroupManagement.AllowedGroupIds.AddRange(ctx.Uow.ToemsUsersGroupsRepository.Get(x => x.UserId == userId).Select(x => x.GroupId).ToList());
                }
            }

            dtoUserGroupManagement.AllowedGroupIds = dtoUserGroupManagement.AllowedGroupIds.Distinct().ToList();

            return dtoUserGroupManagement;

        }

        public DtoUserComputerManagement GetAllowedComputers(int userId)
        {
            var dtoUserComputerManagement = new DtoUserComputerManagement();

            var deniedComputers = new List<int>();
            if (IsAdmin(userId)) return dtoUserComputerManagement;
            
            var allowedGroupIds = new List<int>();
            foreach (var g in GetUsersGroups(userId))
            {
                if (g.EnableComputerGroupAcls)
                {
                    dtoUserComputerManagement.ComputerManagementEnforced = true;
                    allowedGroupIds.AddRange(ctx.UserGroup.GetManagedGroupIds(g.Id));
                    allowedGroupIds.AddRange(ctx.Uow.ToemsUsersGroupsRepository.Get(x => x.UserId == userId).Select(x => x.GroupId).ToList());
                }
            }
            
            foreach(var groupId in allowedGroupIds)
            {
                var groupMembers = ctx.Uow.GroupRepository.GetGroupMembers(groupId);
                dtoUserComputerManagement.AllowedComputerIds.AddRange(groupMembers.Select(x => x.Id));
            }

            dtoUserComputerManagement.AllowedComputerIds = dtoUserComputerManagement.AllowedComputerIds.Distinct().ToList();

            return dtoUserComputerManagement;

        }

        public bool IsAdmin(int userId)
        {
            var user = GetUser(userId);
            if (user.Membership == "Administrator")
                return true;

            foreach(var group in GetUsersGroups(userId))
            {
                if (group.Membership == "Administrator")
                    return true;
            }

            return false;
        }

        public List<EntityToemsUserGroup> GetUsersGroups(int userId)
        {
            var user = GetUser(userId);
            List<EntityToemsUserGroup> usersGroups = new List<EntityToemsUserGroup>();
            if (user == null) return usersGroups;

            var userGroups = ctx.Uow.UserGroupMembershipRepository.Get(x => x.ToemsUserId == userId).ToList();
            foreach(var group in userGroups)
            {
                usersGroups.Add(ctx.Uow.UserGroupRepository.GetById(group.UserGroupId));
            }
            //get legacy group
            if(user.UserGroupId != -1 && !usersGroups.Select(x => x.Id == user.UserGroupId).Any())
            {
                usersGroups.Add(ctx.Uow.UserGroupRepository.GetById(user.UserGroupId));
            }
            return usersGroups;
        }
        public List<AppUser> GetAll()
        {
            return ctx.Uow.UserRepository.Get();
        }

        public bool RemoveUserLegacyGroup(int userId)
        {
            var u = GetUserWithPass(userId);
            if (u == null) return false;
            u.UserGroupId = -1;
            ctx.Uow.UserRepository.Update(u, u.Id);
            ctx.Uow.Save();
            return true;
        }

        public List<AppUser> SearchUsers(DtoSearchFilter filter)
        {
            return ctx.Uow.UserRepository.Search(filter.SearchText);

        }

        public async Task SendLockOutEmail(int userId)
        {
            //Mail not enabled
            if (ctx.Setting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;

            var lockedUser = GetUser(userId);
            foreach (var user in SearchUsers(new DtoSearchFilter()).Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                if (user.Membership != "Administrator" && user.UserId != userId) continue;
                await ctx.Mail.SendMailAsync(lockedUser.UserName + " Has Been Locked For 15 Minutes Because Of Too Many Failed Login Attempts", user.Email, "User Locked");

            }
        }

      






        public string TotalCount()
        {
            return ctx.Uow.UserRepository.Count();
        }

        public DtoActionResult UpdateUser(AppUser user)
        {
            var u = GetUserWithPass(user.UserId);
            if (u == null) return new DtoActionResult { ErrorMessage = "User Not Found", Id = 0 };
            if (GetAdminCount() == 1 && user.Membership != "Administrator" && u.Membership.Equals("Administrator"))
                return new DtoActionResult() {ErrorMessage = "There Must Be At Least 1 Administrator"};
            var validationResult = ValidateUser(user, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {

                    //password has been updated, update the imaging token
                    user.ImagingToken = Guid.NewGuid().ToString("N").ToUpper() + Guid.NewGuid().ToString("N").ToUpper();
                

                ctx.Uow.UserRepository.Update(user, user.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = user.UserId;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateUser(AppUser user, bool isNewUser)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(user.UserName) || !user.UserName.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "User Name Is Not Valid";
                return validationResult;
            }

            if (isNewUser)
            {
                if (ctx.Uow.UserRepository.Exists(h => h.UserName == user.UserName))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A User With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalUser = ctx.Uow.UserRepository.GetById(user.Id);
                if (originalUser.UserName != user.UserName)
                {
                    if (ctx.Uow.UserRepository.Exists(h => h.UserName == user.UserName))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A User With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }
        
        public string GetUserComputerView(int userId)
        {
            var user = GetUser(userId);
            if(user.DefaultComputerView == null)
                return ctx.Setting.GetSettingValue(SettingStrings.DefaultComputerView);
            else if (user.DefaultComputerView.Equals("Default"))
                return ctx.Setting.GetSettingValue(SettingStrings.DefaultComputerView);
            else
                return user.DefaultComputerView;
        }

        public EntityToemsUserOptions GetUserComputerOptions(int userId)
        {
            return ctx.Uow.ToemsUserOptionsRepository.Get().Where(x => x.ToemsUserId == userId).FirstOrDefault();
        }

        public DtoActionResult UpdateOrInsertUserComputerOptions(EntityToemsUserOptions userComputerOptions)
        {
            var actionResult = new DtoActionResult();
            var options = GetUserComputerOptions(userComputerOptions.ToemsUserId);
            if (options == null)
            {
                //insert
                ctx.Uow.ToemsUserOptionsRepository.Insert(userComputerOptions);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = userComputerOptions.Id;

            }
            else
            {
                //update
                userComputerOptions.Id = options.Id;
                ctx.Uow.ToemsUserOptionsRepository.Update(userComputerOptions, userComputerOptions.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = userComputerOptions.Id;
            }

            return actionResult;
        }


        public string GetUserComputerSort(int userId)
        {
            var user = GetUser(userId);

            if (user.ComputerSortMode == null)
                return ctx.Setting.GetSettingValue(SettingStrings.ComputerSortMode);
            else if (user.ComputerSortMode.Equals("Default"))
                return ctx.Setting.GetSettingValue(SettingStrings.ComputerSortMode);
            else
                return user.ComputerSortMode;
        }

        public string GetUserLoginPage(int userId)
        {
            var user = GetUser(userId);
            if (user.DefaultLoginPage == null)
                return ctx.Setting.GetSettingValue(SettingStrings.DefaultLoginPage);
            else if (user.DefaultLoginPage.Equals("Default"))
                return ctx.Setting.GetSettingValue(SettingStrings.DefaultLoginPage);
            else
                return user.DefaultLoginPage;
        }
    }
}