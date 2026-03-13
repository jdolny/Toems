using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ClientApi.Hubs.Authorization
{
    public class HubCertAuth(ServiceContext ctx) : Attribute, IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            var httpContext = invocationContext.Context.GetHttpContext();

            var computerGuid = httpContext.Request.Headers["computerGuid"];
            var certBase64 = httpContext.Request.Headers["certificate"];

            if (string.IsNullOrEmpty(computerGuid) || string.IsNullOrEmpty(certBase64))
                throw new HubException("Unauthorized");

            byte[] bytes = Convert.FromBase64String(certBase64);
            var x509certificate = new X509Certificate2(bytes);

            var computer = ctx.Computer.GetByGuid(computerGuid);

            if (computer == null)
                throw new HubException("Unauthorized");

            if (!ctx.Certificate.ValidateCert(x509certificate))
                throw new HubException("Invalid certificate");

            var deviceCertEntity = ctx.Certificate.GetCertificate(computer.CertificateId);

            var deviceCert = new X509Certificate2(
                deviceCertEntity.PfxBlob,
                ctx.Encryption.DecryptText(deviceCertEntity.Password),
                X509KeyStorageFlags.Exportable);

            if (!deviceCert.Equals(x509certificate))
                throw new HubException("Certificate mismatch");

            return await next(invocationContext);
        }
    }
    
    

    
}