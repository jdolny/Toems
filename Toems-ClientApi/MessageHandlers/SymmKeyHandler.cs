using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ClientApi.MessageHandlers
{
    public class SymmKeyHandler : DelegatingHandler
    {
        private static readonly ILog Logger =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string logId;
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            logId = logId = Guid.NewGuid().ToString("n").Substring(0, 8);
            
            // CATCH THE REQUEST BEFORE SENDING TO THE ROUTING HANDLER
            Logger.Debug($"ID: {logId} - Received decryption request for {request.Headers.GetValues("client").FirstOrDefault()} ");
            Logger.Debug($"ID: {logId} - Request URI {request.RequestUri} ");
            var clientIdentifier = request.Headers.GetValues("identifier").FirstOrDefault();
            var encryptedCert = request.Headers.GetValues("device_cert").FirstOrDefault();

            byte[] keyBytes = null;
            var computerEntity = new ServiceComputer().GetByGuid(clientIdentifier);
            if (computerEntity == null)
            {
                Logger.Debug($"ID: {logId} - Computer With Identity {clientIdentifier} Could Not Be Found.");
            }
            else
            {
                var symmKey = new EncryptionServices().DecryptText(computerEntity.SymmKeyEncrypted);
                keyBytes = Convert.FromBase64String(symmKey);

                var body = request.Content.ReadAsByteArrayAsync().Result;
                if (body != null)
                {
                    if (body.Length != 0)
                    {
                        var decryptedContent = new ServiceSymmetricEncryption().DecryptAsync(keyBytes, body);
                        if (decryptedContent.Status != TaskStatus.Faulted)
                        {
                            Logger.Debug($"ID: {logId} - Successfully decrypted message body");
                            request.Content = new StringContent(decryptedContent.Result, Encoding.UTF8, "application/json");
                            Logger.Debug($"ID: {logId} - Message content: " + decryptedContent.Result);
                        }
                        else
                        {
                            Logger.Debug($"ID: {logId} - Could not decrypt message body");
                        }
                    }
                }
                var decryptedCert = new ServiceSymmetricEncryption().DecryptAsync(keyBytes,
                    Convert.FromBase64String(encryptedCert));

                request.Headers.Remove("device_cert");
                if (decryptedCert.Status != TaskStatus.Faulted)
                {
                    request.Headers.Add("device_cert", decryptedCert.Result);
                    Logger.Debug($"ID: {logId} - Successfully decrypted device certificate");
                    Logger.Debug($"ID: {logId} - Checking certificate authorization");
                }
                else
                {
                    Logger.Debug($"ID: {logId} - Could not decrypt device certificate");
                }
            }

            // SETUP A CALLBACK FOR CATCHING THE RESPONSE - AFTER ROUTING HANDLER, AND AFTER CONTROLLER ACTIVITY
            return base.SendAsync(request, cancellationToken).ContinueWith(
                        task =>
                        {

                            // RETURN THE ORIGINAL RESULT
                            var response = task.Result;
                            //Don't encrypt body if trying to download file
                            if (!request.RequestUri.ToString().EndsWith("GetFile/"))
                            {
                                var body = ResponseHandler(task,keyBytes,computerEntity);
                                if (body != null)
                                    response.Content = body;
                            }
                            
                            return response;
                        }, cancellationToken);
        }

        public StringContent ResponseHandler(Task<HttpResponseMessage> task, byte[] key,EntityComputer computer)
        {
            //encrypt the response back to the client
            //var headers = task.Result.ToString();
            if(task.Result.Content != null)
            {
                Logger.Debug($"ID: {logId} - Encrypting response body");
                var body = task.Result.Content.ReadAsStringAsync().Result;
                Logger.Debug($"ID: {logId} - {body}");
              var encryptedContent = new ServiceSymmetricEncryption().EncryptData(key, body);
                var signature = new ServiceCertificate().SignMessage(Convert.ToBase64String(encryptedContent),
                    computer);
                var jsonResp = new DtoApiStringResponse();
                jsonResp.Value = Convert.ToBase64String(encryptedContent);
                task.Result.Headers.Add("client_signature",Convert.ToBase64String(signature));
                return new StringContent(JsonConvert.SerializeObject(jsonResp), Encoding.UTF8, "application/json");
            }
            else return null;
        }
    }
}