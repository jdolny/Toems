using log4net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;
using TwoFactorAuthNet;

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
        private readonly ILog log = LogManager.GetLogger(typeof(AuthenticationServices));
        public AuthenticationServices()
        {
            _userServices = new ServiceUser();
            _userGroupServices = new ServiceUserGroup();
            _userLockoutServices = new ServiceUserLockout();
        }

        public string ConsoleLogin(string username, string password, string task, string ip)
        {
            log.Info("Console Login Request Received: " + username + " "  + task + " " + ip);
            log.Debug("Password Debug: " + password);
            var result = new Dictionary<string, string>();

            var validationResult = GlobalLogin(username, password, "Console");

            if (!validationResult.Success)
            {
                log.Info("Console Login Request Failed");
                result.Add("valid", "false");
                result.Add("user_id", "");
                result.Add("user_token", "");
            }
            else
            {
                log.Info("Console Login Request Succeeded");
                var ToemsUser = _userServices.GetUserForLogin(username);
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
            var auditLogService = new ServiceAuditLog();
            auditLog.ObjectId = -1;
            auditLog.ObjectName = userName;
            auditLog.UserId = -1;
            auditLog.ObjectType = "User";
            auditLog.AuditType = EnumAuditEntry.AuditType.FailedLogin;

            //Check if user exists in database
            var user = _userServices.GetUserForLogin(userName);
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
                                IsLdapUser = 1,
                                Membership = "User",
                                Theme = "dark",

                                
                            };
                            //Create a local random db pass, should never actually be possible to use.
                            cdUser.Password = Utility.CreatePasswordHash(Utility.GenerateKey(), cdUser.Salt);
                            if (_userServices.AddUser(cdUser).Success)
                            {
                                //add user to group
                                var newUser = _userServices.GetUserForLogin(userName);
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


            //MFA
            if (ServiceSetting.GetSettingValue(SettingStrings.EnableMfa) == "1" && user.MfaSecret == null
                && (user.EnableWebMfa || ServiceSetting.GetSettingValue(SettingStrings.ForceMfa) == "1"
                || user.EnableImagingMfa || ServiceSetting.GetSettingValue(SettingStrings.ForceImagingMfa) == "1"))
            { 
                validationResult.ErrorMessage = "Mfa setup is required"; 
                if(loginType.Equals("Console"))
                {
                    validationResult.Success = false;
                    return validationResult;
                }

                // don't return result yet for web login, still need to check if user actually authenticated correctly to get to the mfa setup page
            }
            else
            {
                if (ServiceSetting.GetSettingValue(SettingStrings.EnableMfa) == "1" && user.MfaSecret != null
                   && loginType.Equals("Console") && (user.EnableImagingMfa || ServiceSetting.GetSettingValue(SettingStrings.ForceImagingMfa) == "1"))
                {
                    var code = password.Substring(password.Length - 6);
                    password = password.Replace(code, "");

                    var mfaResult = new TwoFactorAuth().VerifyCode(new EncryptionServices().DecryptText(user.MfaSecret), code);
                    if (!mfaResult)
                    {
                        auditLog.UserId = user.Id;
                        auditLog.ObjectId = user.Id;
                        auditLogService.AddAuditLog(auditLog);
                        return validationResult;
                    }
                }
                if (ServiceSetting.GetSettingValue(SettingStrings.EnableMfa) == "1" && user.MfaSecret != null
                   && loginType.Equals("Web") && (user.EnableWebMfa || ServiceSetting.GetSettingValue(SettingStrings.ForceMfa) == "1"))

                {
                    var mfaResult = new TwoFactorAuth().VerifyCode(new EncryptionServices().DecryptText(user.MfaSecret), verificationCode);
                    if (!mfaResult)
                    {
                        auditLog.UserId = user.Id;
                        auditLog.ObjectId = user.Id;
                        auditLogService.AddAuditLog(auditLog);
                        return validationResult;
                    }
                }
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
            validationResult.ErrorMessage = "Incorrect Username Or Password";
            auditLog.AuditType = EnumAuditEntry.AuditType.FailedLogin;
            auditLog.UserId = user.Id;
            auditLog.ObjectId = user.Id;
            auditLogService.AddAuditLog(auditLog);
            _userLockoutServices.ProcessBadLogin(user.Id);
            return validationResult;
        }

        public string IpxeLogin(string username, string password, string kernel, string bootImage, string task)
        {
            //no reason to look at other com  servers, just continue to use the currently connected one.
            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (thisComServer == null)
            {
                return null;
            }

            string userToken;
            var consoleRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.ConsoleTasksRequireLogin);
            var globalToken = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingToken);
            if (consoleRequiresLogin.Equals("False"))
                userToken = globalToken;
            else
                userToken = "";

            var webPath = thisComServer.Url + "clientimaging/,";
            var globalComputerArgs = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingArguments);


            var iPxePath = webPath;
            if (iPxePath.Contains("https://")) 
            {
                if (ServiceSetting.GetSettingValue(SettingStrings.IpxeSSL).Equals("False"))
                {
                    iPxePath = iPxePath.ToLower().Replace("https://", "http://");
                    var currentPort = iPxePath.Split(':').Last();
                    iPxePath = iPxePath.Replace(currentPort, ServiceSetting.GetSettingValue(SettingStrings.IpxeHttpPort)) + "/clientimaging/";
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