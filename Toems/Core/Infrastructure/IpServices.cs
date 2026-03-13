namespace Toems_ServiceCore.Infrastructure
{
    public class IpServices(IHttpContextAccessor contextAccessor)
    {
        public string GetIPAddress()
        {
            try
            {
                var context = contextAccessor?.HttpContext;
                if (context == null) return string.Empty;
                var ipAddress = context.Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault();

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    var addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        return addresses[0];
                    }
                }

                return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            }
            catch
            {

                return string.Empty;
            }

        }
    }
}