using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ClientApi.Hubs.Authorization
{
    public class HubCertAuth : AuthorizeAttribute
    {

        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            var computerGuid = request.Headers["computerGuid"];
            var comServer = request.Headers["comServer"];
            var certBase64 = request.Headers["certificate"];
            byte[] bytes = Convert.FromBase64String(certBase64);
            X509Certificate2 x509certificate = new X509Certificate2(bytes);

            var computer = new ServiceComputer().GetByGuid(computerGuid);
            if (computer == null)
                return false;

            if(!new ServiceCertificate().ValidateCert(x509certificate))
            {
                return false;
            }

            var deviceCertEntity = new ServiceCertificate().GetCertificate(computer.CertificateId);
            var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);

            if(!deviceCert.Equals(x509certificate))
            {
                return false;
            }


            return true;
        }

        protected override bool UserAuthorized(System.Security.Principal.IPrincipal user)
        {
            //all users are authorized after the certificate is verified
            return true;
        }

    }
}