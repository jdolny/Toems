using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_Service
{
    /// <summary>
    ///     Summary description for Authenticate
    /// </summary>
    public class AuthenticationServices
    {
        private readonly ServiceUserGroup _userGroupServices;
        private readonly ServiceUserLockout _userLockoutServices;

        private readonly ServiceUser _userServices;

        public AuthenticationServices()
        {
            _userServices = new ServiceUser();
            _userGroupServices = new ServiceUserGroup();
            _userLockoutServices = new ServiceUserLockout();
        }

       

        public DtoValidationResult GlobalLogin(string userName, string password, string loginType)
        {
           
            var validationResult = new DtoValidationResult
            {
                ErrorMessage = "Incorrect Username Or Password",
                Success = false
            };

            var auditLog = new EntityAuditLog();
            var auditLogService = new ServiceAuditLog();
            auditLog.ObjectId = -1;
            auditLog.ObjectName = userName;
            auditLog.UserId = -1;
            auditLog.ObjectType = "User";
            auditLog.AuditType = EnumAuditEntry.AuditType.FailedLogin;

            //Check if user exists in database
            var user = _userServices.GetUser(userName);
            if (user == null)
            {
                //Check For a first time LDAP User Group Login
                if (ServiceSetting.GetSettingValue(SettingStrings.LdapEnabled) == "1")
                {
                    foreach (var ldapGroup in _userGroupServices.GetLdapGroups())
                    {
                        if (new LdapServices().Authenticate(userName, password, ldapGroup.GroupLdapName))
                        {
                            //user is a valid ldap user via ldap group that has not yet logged in.
                            //Add the user and allow login.                         
                            var cdUser = new EntityToemsUser
                            {
                                Name = userName,
                                Salt = Utility.CreateSalt(64),
                                IsLdapUser = 1
                            };
                            //Create a local random db pass, should never actually be possible to use.
                            cdUser.Password = Utility.CreatePasswordHash(Utility.GenerateKey(), cdUser.Salt);
                            if (_userServices.AddUser(cdUser).Success)
                            {
                                //add user to group
                                var newUser = _userServices.GetUser(userName);
                                _userGroupServices.AddNewGroupMember(ldapGroup.Id, newUser.Id);
                                auditLog.UserId = newUser.Id;
                                auditLog.ObjectId = newUser.Id;
                                validationResult.Success = true;
                                auditLog.AuditType = EnumAuditEntry.AuditType.SuccessfulLogin;

                                break;
                            }
                        }
                    }
                }
                auditLogService.AddAuditLog(auditLog);
                return validationResult;
            }

            if (_userLockoutServices.AccountIsLocked(user.Id))
            {
                _userLockoutServices.ProcessBadLogin(user.Id);
                validationResult.ErrorMessage = "Account Is Locked";
                auditLog.UserId = user.Id;
                auditLog.ObjectId = user.Id;
                auditLogService.AddAuditLog(auditLog);
                return validationResult;
            }

            //Check against AD
            if (user.IsLdapUser == 1 && ServiceSetting.GetSettingValue(SettingStrings.LdapEnabled) == "1")
            {
                //Check if user is authenticated against an ldap group
                if (user.UserGroupId != -1)
                {
                    //user is part of a group, is the group an ldap group?
                    var userGroup = _userGroupServices.GetUserGroup(user.UserGroupId);
                    if (userGroup != null)
                    {
                        if (userGroup.IsLdapGroup == 1)
                        {
                            //the group is an ldap group
                            //make sure user is still in that ldap group
                            if (new LdapServices().Authenticate(userName, password, userGroup.GroupLdapName))
                            {
                                validationResult.Success = true;
                            }
                            else
                            {
                                //user is either not in that group anymore, not in the directory, or bad password
                                validationResult.Success = false;

                                if (new LdapServices().Authenticate(userName, password))
                                {
                                    //password was good but user is no longer in the group
                                    //delete the user
                                    _userServices.DeleteUser(user.Id);
                                }
                            }
                        }
                        else
                        {
                            //the group is not an ldap group
                            //still need to check creds against directory
                            if (new LdapServices().Authenticate(userName, password)) validationResult.Success = true;
                        }
                    }
                    else
                    {
                        //group didn't exist for some reason
                        //still need to check creds against directory
                        if (new LdapServices().Authenticate(userName, password)) validationResult.Success = true;
                    }
                }
                else
                {
                    //user is not part of a group, check creds against directory
                    if (new LdapServices().Authenticate(userName, password)) validationResult.Success = true;
                }
            }
            else if (user.IsLdapUser == 1 && ServiceSetting.GetSettingValue(SettingStrings.LdapEnabled) != "1")
            {
                //prevent ldap user from logging in with local pass if ldap auth gets turned off
                validationResult.Success = false;
            }
            //Check against local DB
            else
            {
                var hash = Utility.CreatePasswordHash(password, user.Salt);
                if (user.Password == hash) validationResult.Success = true;
            }

            if (validationResult.Success)
            {
                auditLog.AuditType = EnumAuditEntry.AuditType.SuccessfulLogin;
                auditLog.UserId = user.Id;
                auditLog.ObjectId = user.Id;
                auditLogService.AddAuditLog(auditLog);
                _userLockoutServices.DeleteUserLockouts(user.Id);
                return validationResult;
            }
            auditLog.AuditType = EnumAuditEntry.AuditType.FailedLogin;
            auditLog.UserId = user.Id;
            auditLog.ObjectId = user.Id;
            auditLogService.AddAuditLog(auditLog);
            _userLockoutServices.ProcessBadLogin(user.Id);
            return validationResult;
        }

    }
}