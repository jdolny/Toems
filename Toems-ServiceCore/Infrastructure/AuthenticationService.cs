using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure
{
    public class AuthenticationService(InfrastructureContext ictx, 
        ServiceUserGroup userGroupService,
        ServiceUserGroupMembership userGroupMembershipService,
        ServiceUserLockout userLockoutService, 
        ServiceUser userService,
        ServiceClientComServer clientComServerService)
    {
        public string ConsoleLogin(string username, string password, string task, string ip)
        {
            ictx.Log.Info("Console Login Request Received: " + username + " "  + task + " " + ip);

            var result = new Dictionary<string, string>();

            var validationResult = GlobalLogin(username, password, "Console");

            if (!validationResult.Success)
            {
                ictx.Log.Info("Console Login Request Failed");
                result.Add("valid", "false");
                result.Add("user_id", "");
                result.Add("user_token", "");
            }
            else
            {
                ictx.Log.Info("Console Login Request Succeeded");
                var ToemsUser = userService.GetUserForLogin(username);
                result.Add("valid", "true");
                result.Add("user_id", ToemsUser.Id.ToString());
                result.Add("user_token", ToemsUser.ImagingToken);
            }

            return JsonConvert.SerializeObject(result);
        }

        public DtoValidationResult GlobalLogin(string userName, string password, string loginType, string verificationCode = null)
        {

            var validationResult = new DtoValidationResult
            {
                ErrorMessage = "Incorrect Username Or Password",
                Success = false
            };

            var auditLog = new EntityAuditLog();
            auditLog.ObjectId = -1;
            auditLog.ObjectName = userName;
            auditLog.UserId = -1;
            auditLog.ObjectType = "User";
            auditLog.AuditType = EnumAuditEntry.AuditType.FailedLogin;

            //Check if user exists in database
            var user = userService.GetUserForLogin(userName);
            if (user == null)
            {
                //Check For a first time LDAP User Group Login
                if (ictx.Settings.GetSettingValue(SettingStrings.LdapEnabled) == "1")
                {
                    var userCreated = false;
                    foreach (var ldapGroup in userGroupService.GetLdapGroups())
                    {
                        if (new LdapServices().Authenticate(userName, password, ldapGroup.GroupLdapName))
                        {
                            if (!userCreated)
                            {
                                //user is a valid ldap user via ldap group that has not yet logged in.
                                //Add the user and allow login.                         
                                var cdUser = new EntityToemsUser
                                {
                                    Name = userName,
                                    Salt = Utility.CreateSalt(64),
                                    IsLdapUser = 1,
                                    UserGroupId = -1,
                                    Membership = "User",
                                    Theme = "dark",
                                    DefaultComputerView = "Default",
                                    ComputerSortMode = "Default",
                                    DefaultLoginPage = "Default"
                                };
                                //Create a local random db pass, should never actually be possible to use.
                                cdUser.Password = Utility.CreatePasswordHash(Utility.GenerateKey(), cdUser.Salt);
                                if (userService.AddUser(cdUser).Success)
                                {
                                    userCreated = true;
                                    //add user to group
                                    user = userService.GetUserForLogin(userName);
                                    userGroupMembershipService.AddMembership(new EntityUserGroupMembership() { ToemsUserId = user.Id, UserGroupId = ldapGroup.Id });

                                    auditLog.UserId = user.Id;
                                    auditLog.ObjectId = user.Id;
                                    validationResult.Success = true;
                                    auditLog.AuditType = EnumAuditEntry.AuditType.SuccessfulLogin;
                                }
                            }
                            else
                            {
                                 userGroupMembershipService.AddMembership(new EntityUserGroupMembership() { ToemsUserId = user.Id, UserGroupId = ldapGroup.Id });
                            }
                        }
                    }
                }
                ictx.AuditLog.AddAuditLog(auditLog);
                return validationResult;
            }

            if (userLockoutService.AccountIsLocked(user.Id))
            {
                userLockoutService.ProcessBadLogin(user.Id);
                validationResult.ErrorMessage = "Account Is Locked";
                auditLog.UserId = user.Id;
                auditLog.ObjectId = user.Id;
                ictx.AuditLog.AddAuditLog(auditLog);
                return validationResult;
            }


            //MFA
            if (ictx.Settings.GetSettingValue(SettingStrings.EnableMfa) == "1" && string.IsNullOrEmpty(user.MfaSecret)
                && (user.EnableWebMfa || ictx.Settings.GetSettingValue(SettingStrings.ForceMfa) == "1"
                || user.EnableImagingMfa || ictx.Settings.GetSettingValue(SettingStrings.ForceImagingMfa) == "1"))
            {
                validationResult.ErrorMessage = "Mfa setup is required";
                if (loginType.Equals("Console"))
                {
                    validationResult.Success = false;
                    return validationResult;
                }

                // don't return result yet for web login, still need to check if user actually authenticated correctly to get to the mfa setup page
            }
            else
            {
                if (ictx.Settings.GetSettingValue(SettingStrings.EnableMfa) == "1" && !string.IsNullOrEmpty(user.MfaSecret)
                   && loginType.Equals("Console") && (user.EnableImagingMfa || ictx.Settings.GetSettingValue(SettingStrings.ForceImagingMfa) == "1"))
                {
                    var code = password.Substring(password.Length - 6);
                    password = password.Replace(code, "");

                    var mfaResult = new TwoFactorAuth().VerifyCode(ictx.Encryption.DecryptText(user.MfaSecret), code);
                    if (!mfaResult)
                    {
                        auditLog.UserId = user.Id;
                        auditLog.ObjectId = user.Id;
                        ictx.AuditLog.AddAuditLog(auditLog);
                        return validationResult;
                    }
                }
                if (ictx.Settings.GetSettingValue(SettingStrings.EnableMfa) == "1" && !string.IsNullOrEmpty(user.MfaSecret)
                   && loginType.Equals("Web") && (user.EnableWebMfa || ictx.Settings.GetSettingValue(SettingStrings.ForceMfa) == "1"))

                {
                    var mfaResult = new TwoFactorAuth().VerifyCode(ictx.Encryption.DecryptText(user.MfaSecret), verificationCode);
                    if (!mfaResult)
                    {
                        auditLog.UserId = user.Id;
                        auditLog.ObjectId = user.Id;
                        ictx.AuditLog.AddAuditLog(auditLog);
                        return validationResult;
                    }
                }
            }



            //Check against AD
            if (user.IsLdapUser == 1 && ictx.Settings.GetSettingValue(SettingStrings.LdapEnabled) == "1")
            {
                var ldapGroups = userGroupService.GetLdapGroups();
                if (ldapGroups.Any())
                {
                    validationResult.Success = false;
                    foreach (var ldapGroup in ldapGroups)
                    {
                       
                        if (new LdapServices().Authenticate(userName, password, ldapGroup.GroupLdapName))
                        {
                            //put user back in group if removed at some point

                            userGroupMembershipService.AddMembership(new EntityUserGroupMembership() { ToemsUserId = user.Id, UserGroupId = ldapGroup.Id });
                            validationResult.Success = true;

                        }
                        else
                        {
                            //user is either not in that group anymore, or bad password
                            if (new LdapServices().Authenticate(userName, password))
                            {
                                //password was good but user is no longer in the group
                                //remove user from group
                                userGroupMembershipService.DeleteByIds(user.Id, ldapGroup.Id);
                            }
                        }
                    }
                }
                else
                {
                    //user is not part of an ldap group, check creds against directory
                    if (new LdapServices().Authenticate(userName, password)) validationResult.Success = true;
                }
            }
            else if (user.IsLdapUser == 1 && ictx.Settings.GetSettingValue(SettingStrings.LdapEnabled) != "1")
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

            //update user role based on group membership at each login
            var usersGroups = userService.GetUsersGroups(user.Id);
            if (usersGroups.Any())
            {
                var userNeedsAdminRole = false;
                foreach (var userGroup in usersGroups)
                {
                    if (userGroup.Membership == "Administrator")
                    {
                        userNeedsAdminRole = true;
                        break;
                    }
                }

                user.Membership = userNeedsAdminRole ? "Administrator" : "User";

                userService.UpdateUser(user);
            }

            if (validationResult.Success)
            {
                auditLog.AuditType = EnumAuditEntry.AuditType.SuccessfulLogin;
                auditLog.UserId = user.Id;
                auditLog.ObjectId = user.Id;
                ictx.AuditLog.AddAuditLog(auditLog);
                userLockoutService.DeleteUserLockouts(user.Id);
                return validationResult;
            }
            validationResult.ErrorMessage = "Incorrect Username Or Password";
            auditLog.AuditType = EnumAuditEntry.AuditType.FailedLogin;
            auditLog.UserId = user.Id;
            auditLog.ObjectId = user.Id;
            ictx.AuditLog.AddAuditLog(auditLog);
            userLockoutService.ProcessBadLogin(user.Id);
            return validationResult;
        }

        public string IpxeLogin(string username, string password, string kernel, string bootImage, string task)
        {
            //no reason to look at other com  servers, just continue to use the currently connected one.
            var guid = ictx.Config["ComServerUniqueId"];
            var thisComServer = clientComServerService.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                return null;
            }

            string userToken;
            var consoleRequiresLogin = ictx.Settings.GetSettingValue(SettingStrings.ConsoleTasksRequireLogin);
            var globalToken = ictx.Settings.GetSettingValue(SettingStrings.GlobalImagingToken);
            if (consoleRequiresLogin.Equals("False"))
                userToken = globalToken;
            else
                userToken = "";

            var webPath = thisComServer.Url + "clientimaging/,";
            var globalComputerArgs = ictx.Settings.GetSettingValue(SettingStrings.GlobalImagingArguments);


            var iPxePath = webPath;
            if (iPxePath.Contains("https://")) 
            {
                if (ictx.Settings.GetSettingValue(SettingStrings.IpxeSSL).Equals("False"))
                {
                    iPxePath = iPxePath.ToLower().Replace("https://", "http://");
                    var currentPort = iPxePath.Split(':').Last();
                    iPxePath = iPxePath.Replace(currentPort, ictx.Settings.GetSettingValue(SettingStrings.IpxeHttpPort)) + "/clientimaging/";
                }
                else
                    iPxePath += "clientimaging/";
            }
            else
                iPxePath += "clientimaging/";

            var newLineChar = "\n";

            var validationResult = GlobalLogin(username, password, "iPXE");
            if (!validationResult.Success) return "goto Menu";
            var lines = "#!ipxe" + newLineChar;
            lines += "kernel " + iPxePath + "IpxeBoot?filename=" + kernel +
                     "&type=kernel" +
                     " initrd=" + bootImage + " root=/dev/ram0 rw ramdisk_size=156000 " + " web=" +
                     webPath + " USER_TOKEN=" + userToken +  " consoleblank=0 " +
                     globalComputerArgs + newLineChar;
            lines += "imgfetch --name " + bootImage + " " + iPxePath +
                     "IpxeBoot?filename=" +
                     bootImage + "&type=bootimage" + newLineChar;
            lines += "boot";

            return lines;
        }

    }
}