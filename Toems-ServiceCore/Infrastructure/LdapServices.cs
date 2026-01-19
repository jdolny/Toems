using Toems_Common;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace Toems_ServiceCore.Infrastructure
{
    /// <summary>
    ///     Summary description for Ldap
    /// </summary>
    public class LdapServices(InfrastructureContext ictx)
    {
        public bool Authenticate(string username, string pwd, string ldapGroup = null)
        {
            if (ictx.Settings.GetSettingValue(SettingStrings.LdapEnabled) != "1") return false;

            var path = "LDAP://" + ictx.Settings.GetSettingValue(SettingStrings.LdapServer) + ":" +
                       ictx.Settings.GetSettingValue(SettingStrings.LdapPort) + "/" +
                       ictx.Settings.GetSettingValue(SettingStrings.LdapBaseDN);

            var entry = new DirectoryEntry(path, username, pwd);

            if (ictx.Settings.GetSettingValue(SettingStrings.LdapAuthType) == "Basic")
                entry.AuthenticationType = AuthenticationTypes.None;
            else if (ictx.Settings.GetSettingValue(SettingStrings.LdapAuthType) == "Secure")
                entry.AuthenticationType = AuthenticationTypes.Secure;
            else if (ictx.Settings.GetSettingValue(SettingStrings.LdapAuthType) == "SSL")
                entry.AuthenticationType = AuthenticationTypes.SecureSocketsLayer;
            try
            {
                // Bind to the native AdsObject to force authentication.
                var obj = entry.NativeObject;
                var search = new DirectorySearcher(entry);
                search.Filter = "(" + ictx.Settings.GetSettingValue(SettingStrings.LdapAuthAttribute) + "=" + username +
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
                ictx.Log.Error("Could Not Authenticate User: " + username + " " + ex.Message);
                return false;
            }

          
            return true;
        }
        
        public string GetADGuid(string computerName)
        {
            var domain = Domain.GetCurrentDomain().Name;

            string sADPath = String.Format("LDAP://{0}", domain);
            DirectoryEntry de = new DirectoryEntry(sADPath);

            string sFilter = "(&(objectCategory=computer)(name=" + computerName + "))";
            DirectorySearcher DirectorySearch = new DirectorySearcher(de, sFilter);

            try
            {
                SearchResult DirectorySearchResult = DirectorySearch.FindOne();

                if (null != DirectorySearchResult)
                {
                    DirectoryEntry deComp = DirectorySearchResult.GetDirectoryEntry();
                    ictx.Log.Info("Computer Guid: " + deComp.Guid);
                    return deComp.Guid.ToString();

                }
            }
            catch (Exception ex)
            {
                ictx.Log.Error(String.Format("Active Directory Search Failed: {0}", domain));
                ictx.Log.Error(ex.Message);
                throw;
            }
            return null;
        }
    }
}