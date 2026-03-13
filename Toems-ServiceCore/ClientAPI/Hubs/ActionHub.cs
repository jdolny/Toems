using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Microsoft.AspNetCore.SignalR;
using Toems_ClientApi.Hubs.Authorization;
using Toems_Common.Dto;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;


namespace Toems_ClientApi.Hubs
{
    public class ActionHub(ServiceContext ctx) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;

            var headers = Context.GetHttpContext().Request.Headers;
            var compGuid = headers["computerGuid"];
            var comServer = headers["comServer"];

            ctx.ActiveSocket.AddOrUpdate(compGuid, connectionId, comServer);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;

            ctx.ActiveSocket.Delete(connectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendAction(DtoSocketRequest request)
        {
            var action = new DtoHubAction
            {
                Action = request.action,
                Message = request.message
            };

            await Clients.Clients(request.connectionIds).SendAsync("ClientAction", action);
        }

        public DtoSocketServerVerify VerifyServer()
        {
            var headers = Context.GetHttpContext().Request.Headers;
            var computerGuid = headers["computerGuid"];

            var computer = ctx.Computer.GetByGuid(computerGuid);

            if (computer == null)
                return null;

            var deviceCertEntity = ctx.Certificate.GetCertificate(computer.CertificateId);

            var deviceCert = new X509Certificate2(
                deviceCertEntity.PfxBlob,
                ctx.Encryption.DecryptText(deviceCertEntity.Password),
                X509KeyStorageFlags.Exportable);

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;

            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
            string nonce = Guid.NewGuid().ToString("N");

            string signatureRawData = requestTimeStamp + nonce;

            var rsa = deviceCert.GetRSAPrivateKey();

            byte[] data = Encoding.Unicode.GetBytes(signatureRawData);
            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

            return new DtoSocketServerVerify
            {
                nOnce = nonce,
                Timestamp = requestTimeStamp,
                signature = Convert.ToBase64String(signature)
            };
        }
    }
}