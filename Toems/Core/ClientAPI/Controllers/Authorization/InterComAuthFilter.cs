using log4net;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

using System.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Toems_Common;

using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ClientApi.Controllers.Authorization
{
    public class InterComAuthFilter(ServiceContext ctx, IMemoryCache cache) : IAsyncAuthorizationFilter
    {
        private readonly UInt64 requestMaxAgeInSeconds = 600;
        private readonly string authenticationScheme = "amx";
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var logId = Guid.NewGuid().ToString("n").Substring(0, 8);
            var request = context.HttpContext.Request;

            ctx.Log.Debug($"ID: {logId} - Received intercom auth request");
            ctx.Log.Debug($"ID: {logId} - Request URI {request.Path}");

            var authHeader = request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith(authenticationScheme, StringComparison.OrdinalIgnoreCase))
            {
                ctx.Log.Debug($"ID: {logId} - Authorization header missing");
                context.Result = new UnauthorizedResult();
                return;
            }

            var rawAuthzHeader = authHeader.Substring(authenticationScheme.Length).Trim();
            var headerParts = rawAuthzHeader.Split(':');

            if (headerParts.Length != 4)
            {
                ctx.Log.Debug($"ID: {logId} - Authorization header invalid");
                context.Result = new UnauthorizedResult();
                return;
            }

            var APPId = headerParts[0];
            var incomingSignature = headerParts[1];
            var nonce = headerParts[2];
            var requestTimestamp = headerParts[3];

            var isValid = await IsValidRequest(context.HttpContext, APPId, incomingSignature, nonce, requestTimestamp, logId);

            if (!isValid)
            {
                ctx.Log.Debug($"ID: {logId} - Authorization failed validation");
                context.Result = new UnauthorizedResult();
                return;
            }

            var claims = new[] { new Claim(ClaimTypes.Name, APPId) };
            var identity = new ClaimsIdentity(claims, "InterComAuth");
            context.HttpContext.User = new ClaimsPrincipal(identity);
        }

        private async Task<bool> IsValidRequest(HttpContext httpContext,
            string APPId,
            string incomingSignature,
            string nonce,
            string requestTimestamp,
            string logId)
        {
            var request = httpContext.Request;

            if (IsReplayRequest(nonce, requestTimestamp, logId))
                return false;

            var requestUri = Uri.EscapeDataString($"{request.Scheme}://{request.Host}{request.Path}".ToLower());
            var method = request.Method;

            string requestContentBase64 = "";

            var hash = await ComputeHash(request);

            if (hash != null)
                requestContentBase64 = Convert.ToBase64String(hash);

            var serverKeyBytes =
                Encoding.ASCII.GetBytes(
                    ctx.Encryption.DecryptText(
                        ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted)));

            var sharedKey = Convert.ToBase64String(serverKeyBytes);

            string data = $"{APPId}{method}{requestUri}{requestTimestamp}{nonce}{requestContentBase64}";

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

        private bool IsReplayRequest(string nonce, string requestTimestamp, string logId)
        {
            if (cache.TryGetValue(nonce, out _))
            {
                ctx.Log.Debug($"ID: {logId} - Nonce already used");
                return true;
            }

            DateTime epochStart = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentTs = DateTime.UtcNow - epochStart;

            var serverSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestSeconds = Convert.ToUInt64(requestTimestamp);

            if (requestSeconds > serverSeconds)
                serverSeconds += 300;

            var difference = serverSeconds - requestSeconds;

            if (difference > requestMaxAgeInSeconds)
            {
                ctx.Log.Debug($"ID: {logId} - Request exceeded max age");
                return true;
            }

            cache.Set(nonce, requestTimestamp, TimeSpan.FromSeconds(requestMaxAgeInSeconds));

            return false;
        }

        private static async Task<byte[]> ComputeHash(HttpRequest request)
        {
            if (!request.Body.CanRead)
                return null;

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