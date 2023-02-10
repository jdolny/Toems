using System;
using System.DirectoryServices;
using log4net;
using Toems_Common;
using Toems_Service.Entity;

namespace Toems_Service
{
    /// <summary>
    ///     Summary description for Ldap
    /// </summary>
    public class LdapServices
    {
        private readonly ILog log = LogManager.GetLogger(typeof(LdapServices));

        public bool Authenticate(string username, string pwd, string ldapGroup = null)
        {
            if (ServiceSetting.GetSettingValue(SettingStrings.LdapEnabled) != "1") return false;

            var path = "LDAP://" + ServiceSetting.GetSettingValue(SettingStrings.LdapServer) + ":" +
                       ServiceSetting.GetSettingValue(SettingStrings.LdapPort) + "/" +
                       ServiceSetting.GetSettingValue(SettingStrings.LdapBaseDN);

            var entry = new DirectoryEntry(path, username, pwd);

            if (ServiceSetting.GetSettingValue(SettingStrings.LdapAuthType) == "Basic")
                entry.AuthenticationType = AuthenticationTypes.None;
            else if (ServiceSetting.GetSettingValue(SettingStrings.LdapAuthType) == "Secure")
                entry.AuthenticationType = AuthenticationTypes.Secure;
            else if (ServiceSetting.GetSettingValue(SettingStrings.LdapAuthType) == "SSL")
                entry.AuthenticationType = AuthenticationTypes.SecureSocketsLayer;
            try
            {
                // Bind to the native AdsObject to force authentication.
                var obj = entry.NativeObject;
                var search = new DirectorySearcher(entry);
                search.Filter = "(" + ServiceSetting.GetSettingValue(SettingStrings.LdapAuthAttribute) + "=" + username +
                                ")";
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("memberOf");

                var result = search.FindOne();
                if (result == null)
                {
                    return false;
                }

                if (ldapGroup != null)
                {
                    bool groupMatched = false;
                    foreach(string membership in result.Properties["memberOf"])
                    {
                        var equalsIndex = membership.IndexOf("=", 1);
                        var commaIndex = membership.IndexOf(",", 1);
                        if (equalsIndex == -1)
                        {
                            groupMatched = false;
                        }
                        if (string.Equals(ldapGroup, membership.Substring(equalsIndex + 1,
                            commaIndex - equalsIndex - 1), StringComparison.CurrentCultureIgnoreCase))
                        {
                            groupMatched = true;
                            break;
                        }
                        else
                            groupMatched = false;
                    }

                    return groupMatched;
                  
                }
            }
            catch (Exception ex)
            {
                log.Error("Could Not Authenticate User: " + username + " " + ex.Message);
                return false;
            }

          
            return true;
        }

     
    }
}