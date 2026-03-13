using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;


namespace Toems_ClientApi.Controllers.Authorization
{
   public class CertificateAuthFilter(ServiceContext ctx) : IAsyncAuthorizationFilter
{
    private static readonly ILog Logger =
        LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request;

        string logId = Guid.NewGuid().ToString("n").Substring(0, 8);

        string clientIdentifier = null;
        string deviceCert = null;

        try
        {
            Logger.Debug($"ID: {logId} - Received certificate authentication request");

            clientIdentifier = request.Headers["identifier"].FirstOrDefault();
            deviceCert = request.Headers["device_cert"].FirstOrDefault();
        }
        catch (Exception ex)
        {
            Logger.Debug($"ID: {logId} - Request missing needed headers");
            Logger.Error(ex.Message);

            context.Result = new UnauthorizedResult();
            return;
        }

        if (deviceCert == null || clientIdentifier == null)
        {
            Logger.Debug($"ID: {logId} - Device or client identifier was null");
            context.Result = new UnauthorizedResult();
            return;
        }

        if (await IsValidRequest(clientIdentifier, deviceCert, logId))
        {
            Logger.Debug($"ID: {logId} - Authentication request was successful");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, clientIdentifier)
            };

            var identity = new ClaimsIdentity(claims, "CertificateAuth");
            context.HttpContext.User = new ClaimsPrincipal(identity);
        }
        else
        {
            Logger.Debug($"ID: {logId} - Authentication request failed validation");
            context.Result = new UnauthorizedResult();
        }
    }

    private async Task<bool> IsValidRequest(string computerGuid, string deviceCert, string logId)
    {
        var computerEntity = ctx.Computer.GetByGuid(computerGuid);

        if (computerEntity == null)
        {
            Logger.Debug($"ID: {logId} - Computer with identifier {computerGuid} was not found");
            return false;
        }

        var computerCert = ctx.Certificate.GetCertX509Public(computerEntity.CertificateId);

        if (computerCert == null)
        {
            Logger.Debug($"ID: {logId} - Computer device certificate was null");
            return false;
        }

        var authorizationCert = new X509Certificate2(Convert.FromBase64String(deviceCert));

        if (!computerCert.Equals(authorizationCert))
        {
            Logger.Debug($"ID: {logId} - Certificate mismatch");
            return false;
        }

        if (ctx.Certificate.ValidateCert(authorizationCert))
            return true;

        Logger.Debug($"ID: {logId} - Certificate failed validation");
        return false;
    }
}
}