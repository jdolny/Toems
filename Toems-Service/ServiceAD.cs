using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using log4net;

namespace Toems_Service
{
    public class ServiceAD
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
                    Logger.Info("Computer Guid: " + deComp.Guid);
                    return deComp.Guid.ToString();

                }
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format("Active Directory Search Failed: {0}", domain));
                Logger.Error(ex.Message);
                throw;
            }
            return null;
        }
    }
}
