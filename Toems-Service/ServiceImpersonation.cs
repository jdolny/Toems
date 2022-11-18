using System.Runtime.InteropServices;
using System.Security.Principal;
using System;
using log4net;

namespace Toems_Service
{
    public class ServiceImpersonation : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int LastError { get; private set; }
        public ServiceImpersonation(string domain, string username, string password)
        {
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_INTERACTIVE = 2;

            try
            {
                if (domain == "")
                    domain = Environment.MachineName;

                bool loggedOn = LogonUser(username,
                                            domain,
                                            password,
                                            LOGON32_LOGON_INTERACTIVE,
                                            LOGON32_PROVIDER_DEFAULT,
                                            ref _handle);

                if (!loggedOn)
                {
                    LastError = Marshal.GetLastWin32Error();
                    Logger.Debug("Could Not Impersonate user, error code: " + LastError);
                    return;
                }

                _context = WindowsIdentity.Impersonate(_handle);

            }
            catch (Exception ex)
            {
                Logger.Debug("Could Not Impersonate user: " + ex.Message);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        private bool disposed;
        private WindowsImpersonationContext _context = null;
        private IntPtr _handle = IntPtr.Zero;

        public void Dispose()
        {
            if (!disposed)
            {
                if (_context != null)
                {
                    _context.Undo();
                }

                if (_handle != IntPtr.Zero)
                {
                    CloseHandle(_handle);
                }
            }
            disposed = true;
            GC.SuppressFinalize(this);
        }

        ~ServiceImpersonation()
        {
            Dispose();
        }
    }
}
