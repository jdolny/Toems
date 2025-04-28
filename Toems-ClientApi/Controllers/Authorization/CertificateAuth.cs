using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using log4net;
using Toems_Service.Entity;

namespace Toems_ClientApi.Controllers.Authorization
{
    public class CertificateAuth : Attribute, IAuthenticationFilter
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string logId;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var req = context.Request;
            string clientIdentifier = null;
            string deviceCert = null;
            logId = Guid.NewGuid().ToString("n").Substring(0, 8);
            try
            {
                Logger.Debug($"ID: {logId} - Received certificate authentication request for {req.Headers.GetValues("client").FirstOrDefault()} ");
                Logger.Debug($"ID: {logId} - Request URI {req.RequestUri} ");
                clientIdentifier = req.Headers.GetValues("identifier").FirstOrDefault();
                deviceCert = req.Headers.GetValues("device_cert").FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Debug($"ID: {logId} - Request missing needed headers");
                Logger.Error(ex.Message);
                context.ErrorResult = new UnauthorizedResult(Array.Empty<AuthenticationHeaderValue>(), context.Request);
                return Task.FromResult(0);
            }

            if (deviceCert == null || clientIdentifier == null)
            {
                Logger.Debug($"ID: {logId} - Device or client identifier was null");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            if (isValidRequest(clientIdentifier, deviceCert).Result)
            {
                Logger.Debug($"ID: {logId} - Authentication request was successful");
                var currentPrincipal = new GenericPrincipal(new GenericIdentity(clientIdentifier), null);
                context.Principal = currentPrincipal;
            }
            else
            {
                Logger.Debug($"ID: {logId} - Authentication request failed validation");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }

        public bool AllowMultiple => false;
#pragma warning disable CS1998
        private async Task<bool> isValidRequest(string computerGuid, string deviceCert)
        {
            var computerEntity = new ServiceComputer().GetByGuid(computerGuid);
            if (computerEntity == null)
            {
                Logger.Debug($"ID: {logId} - Computer with identifier {computerGuid} was not found");
                return false;
            }

            var computerCert = new ServiceCertificate().GetCertX509Public(computerEntity.CertificateId);
            if (computerCert == null)
            {
                Logger.Debug($"ID: {logId} - Computer device certificate was null");
                return false;
            }

            var authorizationCert = new X509Certificate2(Convert.FromBase64String(deviceCert));
            if (!computerCert.Equals(authorizationCert))
            {
                Logger.Debug($"ID: {logId} - Certificate mismatch b/w what was sent and current device certificate");
                return false;
            }

            if (new ServiceCertificate().ValidateCert(authorizationCert))
                return true;
            Logger.Debug($"ID: {logId} - Certificate failed validation");
            return false;
        }
#pragma warning restore CS1998
        private class ResultWithChallenge : IHttpActionResult
        {
            private readonly string authenticationScheme = "x509";
            private readonly IHttpActionResult next;

            public ResultWithChallenge(IHttpActionResult next)
            {
                this.next = next;
            }

            public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = await next.ExecuteAsync(cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized) response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(authenticationScheme));

                return response;
            }
        }
    }
}