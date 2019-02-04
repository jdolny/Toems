using log4net;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Toems_Common;
using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ClientApi.Controllers.Authorization
{
    public class InterComAuth : Attribute, IAuthenticationFilter
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UInt64 requestMaxAgeInSeconds = 600;  //10 mins
        private readonly string authenticationScheme = "amx";
        private string logId;
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            logId = logId = Guid.NewGuid().ToString("n").Substring(0, 8);
            var req = context.Request;
            Logger.Debug($"ID: {logId} - Received intercom auth request");
            Logger.Debug($"ID: {logId} - Request URI {req.RequestUri} ");
            if (req.Headers.Authorization != null && authenticationScheme.Equals(req.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                var rawAuthzHeader = req.Headers.Authorization.Parameter;
                Logger.Debug($"ID: {logId} - {rawAuthzHeader}");
                var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);

                if (autherizationHeaderArray != null)
                {
                    var APPId = autherizationHeaderArray[0];
                    var incomingBase64Signature = autherizationHeaderArray[1];
                    Logger.Debug($"ID: {logId} - Received Signature: " + incomingBase64Signature);
                    var nonce = autherizationHeaderArray[2];
                    var requestTimeStamp = autherizationHeaderArray[3];

                    var isValid = isValidRequest(req, APPId, incomingBase64Signature, nonce, requestTimeStamp);

                    if (isValid.Result)
                    {
                        var currentPrincipal = new GenericPrincipal(new GenericIdentity(APPId), null);
                        context.Principal = currentPrincipal;
                    }
                    else
                    {
                        Logger.Debug($"ID: {logId} - Authorization failed validation");
                        context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    }
                }
                else
                {
                    Logger.Debug($"ID: {logId} - Authorization header was null");
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
            else
            {
                Logger.Debug($"ID: {logId} - Authorization header was null");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }

        public bool AllowMultiple
        {
            get { return false; }
        }

        private string[] GetAutherizationHeaderValues(string rawAuthzHeader)
        {

            var credArray = rawAuthzHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }

        }

        private async Task<bool> isValidRequest(HttpRequestMessage req, string APPId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            string requestContentBase64String = "";
            string requestUri = HttpUtility.UrlEncode(req.RequestUri.AbsoluteUri.ToLower());
            string requestHttpMethod = req.Method.Method;


            var serverKeyBytes =
                Encoding.ASCII.GetBytes(
                    new EncryptionServices().DecryptText(
                        ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted)));
            var sharedKey = Convert.ToBase64String(serverKeyBytes);

            if (isReplayRequest(nonce, requestTimeStamp))
            {
                Logger.Debug($"ID: {logId} - Request appears to be a replay, denying {nonce} {requestTimeStamp}");
                return false;
            }

            byte[] hash = await ComputeHash(req.Content);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            string data = String.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);
            Logger.Debug($"ID: {logId} - Expected Signature Data " + data);
            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                var result = (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
                if (!result)
                    Logger.Debug($"ID: {logId} - Signature mismatch");
                Logger.Debug($"ID: {logId} - Expected Signature: " + Convert.ToBase64String(signatureBytes));
                return result;
            }

        }

        private bool isReplayRequest(string nonce, string requestTimeStamp)
        {
            if (System.Runtime.Caching.MemoryCache.Default.Contains(nonce))
            {
                Logger.Debug($"ID: {logId} - This nonce has already been used");
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);
            Logger.Debug($"ID: {logId} - Server Timestamp Seconds " + serverTotalSeconds);
            Logger.Debug($"ID: {logId} - Request Timestamp Seconds " + requestTimeStamp);

            if (requestTotalSeconds > serverTotalSeconds)
            {
                Logger.Debug($"ID: {logId} - Server time is behind client, allowing 5 minute discrepancy");
                //server time is behind client, give it a 5 min window
                serverTotalSeconds += 300;
            }

            var timeStampDifference = serverTotalSeconds - requestTotalSeconds;
            Logger.Debug($"ID: {logId} - Timestamp difference: " + timeStampDifference);
            if (timeStampDifference > requestMaxAgeInSeconds)
            {
                Logger.Debug($"ID: {logId} - Request has exceeded the maximum request age of 600 seconds");
                return true;
            }

            System.Runtime.Caching.MemoryCache.Default.Add(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(requestMaxAgeInSeconds));

            return false;
        }

        private static async Task<byte[]> ComputeHash(HttpContent httpContent)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = await httpContent.ReadAsByteArrayAsync();
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
        }
    }

 
}