﻿using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.Deployment.WindowsInstaller;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using TwoFactorAuthNet;

namespace Toems_Service.Entity
{
    public class ServiceUser
    {
        private readonly UnitOfWork _uow;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ServiceUser()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddUser(EntityToemsUser user)
        {
            user.ImagingToken = Guid.NewGuid().ToString("N").ToUpper() + Guid.NewGuid().ToString("N").ToUpper();
            var validationResult = ValidateUser(user, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.UserRepository.Insert(user);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = user.Id;
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
            _uow.UserRepository.Delete(userId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public int GetAdminCount()
        {
            return Convert.ToInt32(_uow.UserRepository.Count(u => u.Membership == "Administrator"));
        }

        public EntityToemsUser GetUser(int userId)
        {
            var user = _uow.UserRepository.GetById(userId);
            if(user == null) return null;
            user.Password = string.Empty;
            user.Salt = string.Empty;
            user.MfaSecret = string.Empty;
            return user;
        }

        public EntityToemsUser GetUser(string userName)
        {
            var user = _uow.UserRepository.GetFirstOrDefault(u => u.Name == userName);
            if (user == null) return null;
            user.Password = string.Empty;
            user.Salt = string.Empty;
            user.MfaSecret = string.Empty;
            return user;
        }

        public EntityToemsUser GetUserForLogin(string userName)
        {
            var user = _uow.UserRepository.GetFirstOrDefault(u => u.Name == userName);
            if (user == null) return null;
            return user;
        }

        public string GetUserName(int userId)
        {
            return _uow.UserRepository.GetUserName(userId);
        }

        public List<EntityUserRight> GetEffectiveUserRights(int userId)
        {
            var userGroups = _uow.UserGroupMembershipRepository.Get(x => x.ToemsUserId == userId).ToList();

            List<EntityUserGroupRight> allUserGroupRights = new List<EntityUserGroupRight>();
            foreach (var usergroup in userGroups)
            {
                allUserGroupRights.AddRange(_uow.UserGroupRightRepository.Get(x => x.UserGroupId == usergroup.UserGroupId));
            }

            var collectiveUserRights = allUserGroupRights.Select(right => new EntityUserRight { UserId = userId, Right = right.Right }).ToList();

            var userRights = _uow.UserRightRepository.Get(x => x.UserId == userId);
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
                _uow.AuditLogRepository.Get(x => x.UserId == userId && x.AuditType.ToString() == filter.SearchText).OrderByDescending(x => x.Id).Take(filter.Limit).ToList();
            else
            {
                return
                    _uow.AuditLogRepository.Get(x => x.UserId == userId).OrderByDescending(x => x.Id).Take(filter.Limit).ToList();
            }
        }

        public EntityToemsUser GetUserFromToken(string imagingToken)
        {
            var user = _uow.UserRepository.Get(x => x.ImagingToken.Equals(imagingToken)).FirstOrDefault();
            if (user == null)
                return null;
            user.Password = string.Empty;
            user.Salt = string.Empty;
            user.MfaSecret = string.Empty;

            return user;
        }
      

        public DtoApiObjectResponse GetUserForLogin(int userId)
        {
            var result = new DtoApiObjectResponse();
            var user = _uow.UserRepository.GetById(userId);
            if (user != null)
            {
               
                user.Password = string.Empty;
                user.Salt = string.Empty;
                result.Success = true;
                result.Id = user.Id;
                result.ObjectJson = JsonConvert.SerializeObject(user);
            }

            return result;
        }

        public EntityToemsUser GetUserWithPass(int userId)
        {
            return new UnitOfWork().UserRepository.GetById(userId);

        }


        public List<DtoPinnedPolicy> GetPinnedPolicyCounts(int userId)
        {
            return _uow.UserRepository.GetUserPinnedPolicyCounts(userId);
        }

        public List<DtoPinnedGroup> GetPinnedGroupCounts(int userId)
        {

            return _uow.UserRepository.GetUserPinnedGroupCounts(userId);
        }

       
        public List<EntityUserRight> GetUserRights(int userId)
        {
            return _uow.UserRightRepository.Get(x => x.UserId == userId);
        }

        public bool UpdateUsersImagesList(EntityToemsUsersImages image)
        {
            _uow.ToemsUsersImagesRepository.Insert(image);
            _uow.Save();
            return true;
        }

        public bool UpdateUsersGroupsList(EntityToemsUsersGroups group)
        {
            _uow.ToemsUsersGroupsRepository.Insert(group);
            _uow.Save();
            return true;
        }



        public DtoUserImageManagement GetAllowedImages(int userId)
        {
            var dtoUserImageManagement = new DtoUserImageManagement();

            if (IsAdmin(userId)) return dtoUserImageManagement;

            var userGroupServices = new ServiceUserGroup();

            foreach (var g in GetUsersGroups(userId))
            {
                if (g.EnableImageAcls)
                {
                    dtoUserImageManagement.ImageManagementEnforced = true;
                    dtoUserImageManagement.AllowedImageIds.AddRange(userGroupServices.GetManagedImageIds(g.Id));
                    dtoUserImageManagement.AllowedImageIds.AddRange(_uow.ToemsUsersImagesRepository.Get(x => x.UserId == userId).Select(x => x.ImageId).ToList());
                }
            }

            dtoUserImageManagement.AllowedImageIds = dtoUserImageManagement.AllowedImageIds.Distinct().ToList();

            return dtoUserImageManagement;

        }

        public DtoUserGroupManagement GetAllowedGroups(int userId)
        {
            var dtoUserGroupManagement = new DtoUserGroupManagement();

            if (IsAdmin(userId)) return dtoUserGroupManagement;

            var userGroupServices = new ServiceUserGroup();

            foreach (var g in GetUsersGroups(userId))
            {
                if (g.EnableComputerGroupAcls)
                {
                    dtoUserGroupManagement.GroupManagementEnforced = true;
                    dtoUserGroupManagement.AllowedGroupIds.AddRange(userGroupServices.GetManagedGroupIds(g.Id));
                    dtoUserGroupManagement.AllowedGroupIds.AddRange(_uow.ToemsUsersGroupsRepository.Get(x => x.UserId == userId).Select(x => x.GroupId).ToList());
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

            var userGroupServices = new ServiceUserGroup();
            var allowedGroupIds = new List<int>();
            foreach (var g in GetUsersGroups(userId))
            {
                if (g.EnableComputerGroupAcls)
                {
                    dtoUserComputerManagement.ComputerManagementEnforced = true;
                    allowedGroupIds.AddRange(userGroupServices.GetManagedGroupIds(g.Id));
                    allowedGroupIds.AddRange(_uow.ToemsUsersGroupsRepository.Get(x => x.UserId == userId).Select(x => x.GroupId).ToList());
                }
            }

            var groupServices = new ServiceGroup();

            foreach(var groupId in allowedGroupIds)
            {
                dtoUserComputerManagement.AllowedComputerIds.AddRange(groupServices.GetGroupMembers(groupId).Select(x => x.Id));
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

            var userGroups = _uow.UserGroupMembershipRepository.Get(x => x.ToemsUserId == userId).ToList();
            foreach(var group in userGroups)
            {
                usersGroups.Add(_uow.UserGroupRepository.GetById(group.UserGroupId));
            }
            //get legacy group
            if(user.UserGroupId != -1 && !usersGroups.Select(x => x.Id == user.UserGroupId).Any())
            {
                usersGroups.Add(_uow.UserGroupRepository.GetById(user.UserGroupId));
            }
            return usersGroups;
        }
        public List<EntityToemsUser> GetAll()
        {
            var users = _uow.UserRepository.Get();
            foreach(var user in users)
            {
                user.Password = string.Empty;
                user.Salt = string.Empty;
                user.MfaSecret = string.Empty;
            }
            return users;
        }

        public bool RemoveUserLegacyGroup(int userId)
        {
            var u = GetUserWithPass(userId);
            if (u == null) return false;
            u.UserGroupId = -1;
            _uow.UserRepository.Update(u, u.Id);
            _uow.Save();
            return true;
        }

        public List<EntityToemsUser> SearchUsers(DtoSearchFilter filter)
        {
            var users = _uow.UserRepository.Search(filter.SearchText);
            foreach (var user in users)
            {
                user.Password = string.Empty;
                user.Salt = string.Empty;
                user.MfaSecret = string.Empty;
            }
            return users;
        }

        public void SendLockOutEmail(int userId)
        {
            //Mail not enabled
            if (ServiceSetting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;

            var lockedUser = GetUser(userId);
            foreach (var user in SearchUsers(new DtoSearchFilter()).Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                if (user.Membership != "Administrator" && user.Id != userId) continue;
                var mail = new MailServices
                {
                    MailTo = user.Email,
                    Body = lockedUser.Name + " Has Been Locked For 15 Minutes Because Of Too Many Failed Login Attempts",
                    Subject = "User Locked"
                };
                mail.Send();
            }
        }

      
        public bool ResetUserMfaData(int userId)
        {
            var u = GetUserWithPass(userId);
            if (u == null) return false;
            u.MfaSecret = null;
            _uow.UserRepository.Update(u, u.Id);
            _uow.Save();
            return true;
        }

        public bool CheckMfaSetupComplete(int userId)
        {
            var u = GetUserWithPass(userId);
            if (u == null) return false;
            if(!string.IsNullOrEmpty(u.MfaSecret))
                return true;
            if (ServiceSetting.GetSettingValue(SettingStrings.EnableMfa) == "1" && string.IsNullOrEmpty(u.MfaSecret)
                 && (u.EnableWebMfa || ServiceSetting.GetSettingValue(SettingStrings.ForceMfa) == "1"))
                return false;

            return true;
        }

        public bool VerifyMfaSecret(int userId, string code)
        {
            var u = GetUserWithPass(userId);
            if (u == null) return false;
            var result = new TwoFactorAuth().VerifyCode(new EncryptionServices().DecryptText(u.MfaTempSecret), code);
            if (result)
            {
                u.MfaSecret = u.MfaTempSecret;
                u.MfaTempSecret = null;
                _uow.UserRepository.Update(u, u.Id);
                _uow.Save();
                return true;
            }

            return false;
        }

        public string GenerateTempMfaSecret(int userId)
        {
            var u = GetUserWithPass(userId);
            var mfa = new TwoFactorAuth("Theopenem", 6, 30, Algorithm.SHA1, new ToemsQrProvider());
            
            var secret = mfa.CreateSecret(160);
            u.MfaTempSecret = new EncryptionServices().EncryptText(secret);
            _uow.UserRepository.Update(u, u.Id);
            _uow.Save();
            return mfa.GetQrCodeImageAsDataUri(u.Name, secret);
        }

        public string TotalCount()
        {
            return _uow.UserRepository.Count();
        }

        public DtoActionResult UpdateUser(EntityToemsUser user)
        {
            var u = GetUserWithPass(user.Id);
            if (u == null) return new DtoActionResult { ErrorMessage = "User Not Found", Id = 0 };
            if (GetAdminCount() == 1 && user.Membership != "Administrator" && u.Membership.Equals("Administrator"))
                return new DtoActionResult() {ErrorMessage = "There Must Be At Least 1 Administrator"};
            var validationResult = ValidateUser(user, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                if(string.IsNullOrEmpty(user.Password) && string.IsNullOrEmpty(user.Salt))
                {
                    user.Salt = u.Salt;
                    user.Password = u.Password;
                }
                else
                {
                    //password has been updated, update the imaging token
                    user.ImagingToken = Guid.NewGuid().ToString("N").ToUpper() + Guid.NewGuid().ToString("N").ToUpper();
                }
                if (string.IsNullOrEmpty(user.MfaSecret))
                    user.MfaSecret = u.MfaSecret;
                _uow.UserRepository.Update(user, user.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = user.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateUser(EntityToemsUser user, bool isNewUser)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(user.Name) || !user.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "User Name Is Not Valid";
                return validationResult;
            }

            if (isNewUser)
            {
                if (string.IsNullOrEmpty(user.Password))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "Password Is Not Valid";
                    return validationResult;
                }

                if (_uow.UserRepository.Exists(h => h.Name == user.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A User With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalUser = _uow.UserRepository.GetById(user.Id);
                if (originalUser.Name != user.Name)
                {
                    if (_uow.UserRepository.Exists(h => h.Name == user.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A User With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

        public List<DtoProcessWithTime> GetUserProcessTimes(DateTime dateCutoff, int limit, string userName)
        {
            return new ReportRepository().GetTopProcessTimesForUser(dateCutoff, limit, userName);
        }

        public List<DtoProcessWithCount> GetUserProcessCounts(DateTime dateCutoff, int limit, string userName)
        {
            return new ReportRepository().GetTopProcessCountsForUser(dateCutoff, limit, userName);
        }

        public List<DtoProcessWithUser> GetAllProcessForUser(DateTime dateCutoff, int limit, string userName)
        {
            return new ReportRepository().GetAllProcessForUser(dateCutoff, limit, userName);
        }

        public string GetUserComputerView(int userId)
        {
            var user = GetUser(userId);
            if(user.DefaultComputerView == null)
                return ServiceSetting.GetSettingValue(SettingStrings.DefaultComputerView);
            else if (user.DefaultComputerView.Equals("Default"))
                return ServiceSetting.GetSettingValue(SettingStrings.DefaultComputerView);
            else
                return user.DefaultComputerView;
        }

        public EntityToemsUserOptions GetUserComputerOptions(int userId)
        {
            return _uow.ToemsUserOptionsRepository.Get().Where(x => x.ToemsUserId == userId).FirstOrDefault();
        }

        public DtoActionResult UpdateOrInsertUserComputerOptions(EntityToemsUserOptions userComputerOptions)
        {
            var actionResult = new DtoActionResult();
            var options = GetUserComputerOptions(userComputerOptions.ToemsUserId);
            if (options == null)
            {
                //insert
                _uow.ToemsUserOptionsRepository.Insert(userComputerOptions);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = userComputerOptions.Id;

            }
            else
            {
                //update
                userComputerOptions.Id = options.Id;
                _uow.ToemsUserOptionsRepository.Update(userComputerOptions, userComputerOptions.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = userComputerOptions.Id;
            }

            return actionResult;
        }


        public string GetUserComputerSort(int userId)
        {
            var user = GetUser(userId);

            if (user.ComputerSortMode == null)
                return ServiceSetting.GetSettingValue(SettingStrings.ComputerSortMode);
            else if (user.ComputerSortMode.Equals("Default"))
                return ServiceSetting.GetSettingValue(SettingStrings.ComputerSortMode);
            else
                return user.ComputerSortMode;
        }

        public string GetUserLoginPage(int userId)
        {
            var user = GetUser(userId);
            if (user.DefaultLoginPage == null)
                return ServiceSetting.GetSettingValue(SettingStrings.DefaultLoginPage);
            else if (user.DefaultLoginPage.Equals("Default"))
                return ServiceSetting.GetSettingValue(SettingStrings.DefaultLoginPage);
            else
                return user.DefaultLoginPage;
        }
    }
}