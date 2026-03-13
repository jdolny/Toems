using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Toems_ServiceCore.Infrastructure
{
    public class ServiceImpersonation(ServiceContext ctx) : IDisposable
    {
        private IntPtr _handle = IntPtr.Zero;
        private bool _disposed;

        public int LastError { get; private set; }

        public T RunAs<T>(string domain, string username, string password, Func<T> func)
        {
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_NEW_CREDENTIALS = 9; // better for remote

            if (string.IsNullOrEmpty(domain))
                domain = Environment.MachineName;

            bool loggedOn = LogonUser(username,
                domain,
                password,
                LOGON32_LOGON_NEW_CREDENTIALS,
                LOGON32_PROVIDER_DEFAULT,
                ref _handle);

            if (!loggedOn)
            {
                LastError = Marshal.GetLastWin32Error();
                ctx.Log.Debug("Could not impersonate user, error code: " + LastError);
                throw new InvalidOperationException($"Impersonation failed. Win32: {LastError}");
            }

            using var identity = new WindowsIdentity(_handle);

            return WindowsIdentity.RunImpersonated(identity.AccessToken, func);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_handle != IntPtr.Zero)
                {
                    CloseHandle(_handle);
                }

                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~ServiceImpersonation() => Dispose();
    }
}
