//http://www.codeproject.com/Articles/43091/Connect-to-a-UNC-Path-with-Credentials

using System.Runtime.InteropServices;
using log4net;
using Toems_Common;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore
{
    public class UncServices(EncryptionServices encryption, ILog log, ServiceSetting settings) : IDisposable
    {
        private bool disposed;
        private string sDomain;
        private string sPassword;

        private string sUNCPath;
        private string sUser;

        /// <summary>
        ///     The last system error code returned from NetUseAdd or NetUseDel.  Success = 0
        /// </summary>
        public int LastError { get; private set; }

        public void Dispose()
        {
            if (!this.disposed)
            {
                NetUseDelete();
            }
            disposed = true;
            GC.SuppressFinalize(this);
        }

        ~UncServices()
        {
            Dispose();
        }

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint NetUseAdd(
            string UncServerName,
            uint Level,
            ref USE_INFO_2 Buf,
            out uint ParmError);

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint NetUseDel(
            string UncServerName,
            string UseName,
            uint ForceCond);

        /// <summary>
        ///     Ends the connection to the remote resource
        /// </summary>
        /// <returns>True if it succeeds.  Use LastError to get the system error code</returns>
        public bool NetUseDelete()
        {
            uint returncode;
            try
            {
                returncode = NetUseDel(null, sUNCPath, 2);
                LastError = (int) returncode;
                return returncode == 0;
            }
            catch
            {
                LastError = Marshal.GetLastWin32Error();
                return false;
            }
        }

        /// <summary>
        ///     Connects to a UNC path using the credentials supplied.
        /// </summary>
        /// <param name="UNCPath">Fully qualified domain name UNC path</param>
        /// <param name="User">A user with sufficient rights to access the path.</param>
        /// <param name="Domain">Domain of User.</param>
        /// <param name="Password">Password of User</param>
        /// <returns>True if mapping succeeds.  Use LastError to get the system error code.</returns>
        public bool NetUseWithCredentials(string UNCPath, string User, string Domain, string Password)
        {
            sUNCPath = UNCPath;
            sUser = User;
            sPassword = Password;
            sDomain = Domain;
            return NetUseWithCredentials();
        }

        public bool ConnectWithCredentials(string UNCPath, string User, string Domain, string Password)
        {
            sUNCPath = UNCPath.TrimEnd('\\');
            sUser = User;
            sPassword = Password;
            sDomain = Domain;
            return Connect();
        }

        public bool Connect()
        {
            uint returncode;
            try
            {
                var useinfo = new USE_INFO_2();

                useinfo.ui2_remote = sUNCPath;
                useinfo.ui2_username = sUser;
                useinfo.ui2_domainname = sDomain;
                useinfo.ui2_password = sPassword;
                useinfo.ui2_asg_type = 0;
                useinfo.ui2_usecount = 1;
                uint paramErrorIndex;
                returncode = NetUseAdd(null, 2, ref useinfo, out paramErrorIndex);
                LastError = (int)returncode;
                if (returncode != 1219 && returncode != 0)
                {
                    log.Error("Could Not Connect To: " + sUNCPath);
                    log.Error("Error Code: " + returncode);
                }
                return returncode == 0;
            }
            catch (Exception ex)
            {
                LastError = Marshal.GetLastWin32Error();
                log.Error("Could Not Connect To Share: " + sUNCPath);
                log.Error(ex.Message);
                return false;
            }
        }

        public bool NetUseWithCredentials()
        {
            if(settings.GetSetting(SettingStrings.StorageType).Value == "Local")
            return true;
            sUNCPath = settings.GetSetting(SettingStrings.StoragePath).Value.TrimEnd('\\'); //dont' know why, but mount fails if path ends with \
            sUser = settings.GetSetting(SettingStrings.StorageUsername).Value;
            sPassword =
                encryption.DecryptText(settings.GetSetting(SettingStrings.StoragePassword).Value);
            sDomain = settings.GetSetting(SettingStrings.StorageDomain).Value;
            uint returncode;
            try
            {
                var useinfo = new USE_INFO_2();

                useinfo.ui2_remote = sUNCPath;
                useinfo.ui2_username = sUser;
                useinfo.ui2_domainname = sDomain;
                useinfo.ui2_password = sPassword;
                useinfo.ui2_asg_type = 0;
                useinfo.ui2_usecount = 1;
                uint paramErrorIndex;
                returncode = NetUseAdd(null, 2, ref useinfo, out paramErrorIndex);
                LastError = (int) returncode;
                if (returncode != 1219 && returncode != 0)
                {
                    log.Error("Could Not Connect To Storage Location: " + sUNCPath);
                    log.Error("Error Code: " + returncode);
                }
                return returncode == 0;
            }
            catch (Exception ex)
            {
                LastError = Marshal.GetLastWin32Error();
                log.Error("Could Not Connect To Share");
                log.Error(ex.Message);
                return false;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct USE_INFO_2
        {
            internal string ui2_local;
            internal string ui2_remote;
            internal string ui2_password;
            internal uint ui2_status;
            internal uint ui2_asg_type;
            internal uint ui2_refcount;
            internal uint ui2_usecount;
            internal string ui2_username;
            internal string ui2_domainname;
        }
    }
}