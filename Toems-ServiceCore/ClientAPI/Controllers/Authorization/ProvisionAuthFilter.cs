using log4net;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Toems_Common;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;


namespace Toems_ClientApi.Controllers.Authorization
{
    public class ProvisionAuthFilter(ServiceContext ctx, IMemoryCache cache) : IAsyncAuthorizationFilter
    {
        private readonly UInt64 requestMaxAgeInSeconds = 600;
        private readonly string authenticationScheme = "amx";
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var logId = Guid.NewGuid().ToString("n").Substring(0, 8);
            var request = context.HttpContext.Request;

            ctx.Log.Debug($"ID: {logId} - Received provision auth request");
            ctx.Log.Debug($"ID: {logId} - Request URI {request.Path}");

            var authHeader = request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith(authenticationScheme, StringComparison.OrdinalIgnoreCase))
            {
                ctx.Log.Debug($"ID: {logId} - Authorization header missing");
                context.Result = new UnauthorizedResult();
                return;
            }

            var rawHeader = authHeader.Substring(authenticationScheme.Length).Trim();
            var parts = rawHeader.Split(':');

            if (parts.Length != 4)
            {
                ctx.Log.Debug($"ID: {logId} - Authorization header invalid");
                context.Result = new UnauthorizedResult();
                return;
            }

            var APPId = parts[0];
            var incomingSignature = parts[1];
            var nonce = parts[2];
            var timestamp = parts[3];

            var valid = await IsValidRequest(context.HttpContext, APPId, incomingSignature, nonce, timestamp, logId);

            if (!valid)
            {
                ctx.Log.Debug($"ID: {logId} - Authorization failed validation");
                context.Result = new UnauthorizedResult();
                return;
            }

            var claims = new[] { new Claim(ClaimTypes.Name, APPId) };
            var identity = new ClaimsIdentity(claims, "ProvisionAuth");
            context.HttpContext.User = new ClaimsPrincipal(identity);
        }

        private async Task<bool> IsValidRequest(HttpContext httpContext,
            string APPId,
            string incomingSignature,
            string nonce,
            string timestamp,
            string logId)
        {
            if (IsReplayRequest(nonce, timestamp, logId))
                return false;

            var request = httpContext.Request;

            var requestUri =
                Uri.EscapeDataString($"{request.Scheme}://{request.Host}{request.Path}".ToLower());

            var method = request.Method;

            string requestContentBase64 = "";

            var hash = await ComputeHash(request);

            if (hash != null)
                requestContentBase64 = Convert.ToBase64String(hash);

            var serverKeyBytes =
                Encoding.ASCII.GetBytes(
                    ctx.Encryption.DecryptText(
                        ctx.Setting.GetSettingValue(SettingStrings.ProvisionKeyEncrypted)));

            var sharedKey = Convert.ToBase64String(serverKeyBytes);

            string data =
                $"{APPId}{method}{requestUri}{timestamp}{nonce}{requestContentBase64}";

            ctx.Log.Debug($"ID: {logId} - Expected Signature Data {data}");

            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            using var hmac = new HMACSHA256(secretKeyBytes);

            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

            var expectedSignature = Convert.ToBase64String(signatureBytes);

            if (!incomingSignature.Equals(expectedSignature, StringComparison.Ordinal))
            {
                ctx.Log.Debug($"ID: {logId} - Signature mismatch");
                return false;
            }

            ctx.Log.Debug($"ID: {logId} - Expected Signature: {expectedSignature}");

            return true;
        }

        private bool IsReplayRequest(string nonce, string timestamp, string logId)
        {
            if (cache.TryGetValue(nonce, out _))
            {
                ctx.Log.Debug($"ID: {logId} - Nonce already used");
                return true;
            }

            DateTime epochStart = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentTs = DateTime.UtcNow - epochStart;

            var serverSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestSeconds = Convert.ToUInt64(timestamp);

            if (requestSeconds > serverSeconds)
            {
                ctx.Log.Debug($"ID: {logId} - Server time behind client");
                serverSeconds += 300;
            }

            var diff = serverSeconds - requestSeconds;

            if (diff > requestMaxAgeInSeconds)
            {
                ctx.Log.Debug($"ID: {logId} - Request exceeded max age");
                return true;
            }

            cache.Set(nonce, timestamp, TimeSpan.FromSeconds(requestMaxAgeInSeconds));

            return false;
        }

        private static async Task<byte[]> ComputeHash(HttpRequest request)
        {
            request.EnableBuffering();

            using var md5 = MD5.Create();
            using var ms = new MemoryStream();

            await request.Body.CopyToAsync(ms);
            request.Body.Position = 0;

            var content = ms.ToArray();

            if (content.Length == 0)
                return null;

            return md5.ComputeHash(content);
        }
    }
}
