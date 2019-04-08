using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Microsoft.AspNet.SignalR;
using Toems_ClientApi.Hubs.Authorization;
using Toems_Common.Dto;
using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ClientApi.Hubs
{
    [HubCertAuth]
    public class ActionHub : Hub
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public override Task OnConnected()
        {
            string connectionId = Context.ConnectionId;
            var compGuid = Context.Headers["computerGuid"];
            var comServer = Context.Headers["comServer"];
            new ServiceActiveSocket().AddOrUpdate(compGuid, connectionId, comServer);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            new ServiceActiveSocket().Delete(connectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string connectionId = Context.ConnectionId;
            var compGuid = Context.Headers["computerGuid"];
            var comServer = Context.Headers["comServer"];
            new ServiceActiveSocket().AddOrUpdate(compGuid, connectionId, comServer);
            return base.OnReconnected();
        }

        public void SendAction(DtoSocketRequest request)
        {
            var action = new DtoHubAction();
            action.Action = request.action;
            action.Message = request.message;

            var context = GlobalHost.ConnectionManager.GetHubContext<ActionHub>();
            context.Clients.Clients(request.connectionIds).ClientAction(action);
        }

        public DtoSocketServerVerify VerifyServer()
        {
            var computerGuid = Context.Headers["computerGuid"];
            var computer = new ServiceComputer().GetByGuid(computerGuid);
            if (computer == null)
                return null;

            var deviceCertEntity = new ServiceCertificate().GetCertificate(computer.CertificateId);
            var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);

            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            string nonce = Guid.NewGuid().ToString("N");
            string signatureRawData = requestTimeStamp + nonce;
            var csp = (RSACryptoServiceProvider)deviceCert.PrivateKey;
            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(signatureRawData);
            byte[] hash = sha1.ComputeHash(data);
            var signature = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));

            var serverVerification = new DtoSocketServerVerify();
            serverVerification.nOnce = nonce;
            serverVerification.Timestamp = requestTimeStamp;
            serverVerification.signature = Convert.ToBase64String(signature);
            return serverVerification;
        }
    }
}