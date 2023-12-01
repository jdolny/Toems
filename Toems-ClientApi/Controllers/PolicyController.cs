using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{

    public class PolicyController : ApiController
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [CertificateAuth]
        [HttpPost]
        public DtoTriggerResponse GetClientPolicies(DtoPolicyRequest policyRequest)
        {
            return new GetClientPolicies().Execute(policyRequest);
        }

        [CertificateAuth]
        [HttpPost]
        public DtoActionResult AddHistory(DtoPolicyResults results)
        {
            var clientGuid = RequestContext.Principal.Identity.Name;
            return new ServicePolicyHistory().AddHistories(results, clientGuid);

        }

        [CertificateAuth]
        [HttpPost]
        public DtoActionResult UpdateLastSocketResult(DtoApiStringResponse result)
        {
            var clientGuid = RequestContext.Principal.Identity.Name;
            return new ServiceComputer().UpdateSocketResult(result.Value, clientGuid);

        }

        [CertificateAuth]
        [HttpGet]
        public List<EntityWingetModule> GetComputerWingetModules()
        {
            var clientGuid = RequestContext.Principal.Identity.Name;
            return new ServiceComputer().GetComputerWingetUpgrades(clientGuid);

        }

        [CertificateAuth]
        [HttpGet]
        public DtoApiStringResponse GetRemotelyInstallArgs()
        {
            return new DtoApiStringResponse() { Value = new ServiceRemoteAccess().GetRemotelyInstallArgs() };
        }


        [CertificateAuth]
        [HttpPost]
        public DtoActionResult UpdateRemoteAccessId(DtoRemotelyConnectionInfo conInfo)
        {
            var clientGuid = RequestContext.Principal.Identity.Name;
            return new ServiceRemoteAccess().UpdateComputerRemoteAccessId(conInfo, clientGuid);
        }

        [ProvisionAuth]
        [HttpPost]
        public DtoDownloadConnectionResult CreateDownloadConnection(DtoDownloadConRequest conRequest)
        {
            var result = new DtoDownloadConnectionResult();

            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (thisComServer == null)
            {
                Logger.Error($"Com Server With Guid {guid} Not Found");
                result.ErrorMessage = $"Com Server With Guid {guid} Not Found";
                return result;
            }

           
            if (!int.TryParse(thisComServer.EmMaxClients.ToString(), out var maxClientConnections))
            {
                result.ErrorMessage = "Could Not Determine The MaxClientConnections For The Com Server: " +
                                      conRequest.ComServer;
                return result;
            }

            if (maxClientConnections == 0) // zero is unlimited
            {
                result.Success = true;
                result.QueueIsFull = false;
                return result;
            }
           
            var client = new ServiceComputer().GetByGuid(conRequest.ComputerGuid);
            if (client == null)
            {
                result.ErrorMessage = "Could Not Find Computer With Guid: " + RequestContext.Principal.Identity.Name;
                return result;
            }

          

            var serviceCurrentDownloads = new ServiceCurrentDownload();
            var currentConnections = serviceCurrentDownloads.TotalCount(conRequest.ComServer);
            var activeClient = serviceCurrentDownloads.GetByClientId(client.Id,conRequest.ComServer);
            if (activeClient == null && (currentConnections >= maxClientConnections))
            {
                var activeCountAfterExpire = serviceCurrentDownloads.ExpireOldConnections();
                if (activeCountAfterExpire >= maxClientConnections)
                {
                    result.Success = true;
                    result.QueueIsFull = true;
                    return result;
                }
            }

            //add new download connection for this client
            if (activeClient == null)
            {
                var currentDownload = new EntityCurrentDownload();
                currentDownload.ComputerId = client.Id;
                currentDownload.ComServer = conRequest.ComServer;
                serviceCurrentDownloads.Add(currentDownload);
                result.Success = true;
                result.QueueIsFull = false;
            }
            //update time for existing download connection
            else
            {
                activeClient.LastRequestTimeLocal = DateTime.Now;
                serviceCurrentDownloads.Update(activeClient);
                result.Success = true;
                result.QueueIsFull = false;
            }

            return result;
        }

        [CertificateAuth]
        [HttpPost]
        public HttpResponseMessage GetFile(DtoClientFileRequest fileRequest)
        {
            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if(thisComServer == null)
            {
                Logger.Error($"Com Server With Guid {guid} Not Found");
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            var maxBitRate = thisComServer.EmMaxBps;
            var basePath = thisComServer.LocalStoragePath;

            var fullPath = Path.Combine(basePath, "software_uploads", fileRequest.ModuleGuid,
                fileRequest.FileName);
            if (File.Exists(fullPath))
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                try
                {
                    var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    if(maxBitRate == 0)
                        result.Content = new StreamContent(stream);
                    else
                    {
                        Stream throttledStream = new ThrottledStream(stream, maxBitRate);
                        result.Content = new StreamContent(throttledStream);
                    }
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = fileRequest.FileName;
                    return result;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);

                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [ProvisionAuth]
        [HttpPost]
        public HttpResponseMessage GetClientMsi(DtoClientFileRequest fileRequest)
        {
            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (thisComServer == null)
            {
                Logger.Error($"Com Server With Guid {guid} Not Found");
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            var maxBitRate = thisComServer.EmMaxBps;
            var basePath = thisComServer.LocalStoragePath;

            var fullPath = Path.Combine(basePath, "client_versions", fileRequest.FileName);
            if (File.Exists(fullPath))
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                try
                {
                    var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    if (maxBitRate == 0)
                        result.Content = new StreamContent(stream);
                    else
                    {
                        Stream throttledStream = new ThrottledStream(stream, maxBitRate);
                        result.Content = new StreamContent(throttledStream);
                    }
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = fileRequest.FileName;
                    return result;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);

                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [CertificateAuth]
        public DtoApiStringResponse GetScript(string moduleGuid)
        {
            var script = new ServiceScriptModule().GetModuleByGuid(moduleGuid);
            return new DtoApiStringResponse() { Value = script.ScriptContents };

        }

        [CertificateAuth]
        [HttpGet]
        public DtoApiBoolResponse AddToFirstRunGroup()
        {
            var clientGuid = RequestContext.Principal.Identity.Name;
            var client = new ServiceComputer().GetByGuid(clientGuid);
            if (client == null)
                return new DtoApiBoolResponse() { Value = false };
            var membership = new EntityGroupMembership();
            membership.ComputerId = client.Id;
            membership.GroupId = -2;

            new ServiceGroupMembership().AddMembership(new List<EntityGroupMembership>() { membership });
            return new DtoApiBoolResponse() { Value = true };
        }

        [CertificateAuth]
        [HttpGet]
        public DtoApiBoolResponse RemoveFromFirstRunGroup()
        {
            var clientGuid = RequestContext.Principal.Identity.Name;
            var client = new ServiceComputer().GetByGuid(clientGuid);
            if (client == null)
                return new DtoApiBoolResponse() { Value = false };

            new ServiceGroup().DeleteMembership(client.Id, -2);
            return new DtoApiBoolResponse() { Value = true };
        }

        [CertificateAuth]
        public DtoDomainJoinCredentials GetDomainJoinCredentials()
        {
            var enableImpersonationAccess = ConfigurationManager.AppSettings["EnableImpersonationAccess"];
            if (!enableImpersonationAccess.ToLower().Equals("true"))
            {
                Logger.Debug("Retrieving Domain Credentials For This Com Server Is Disabled");
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden));
            }
            return new ServiceSetting().GetDomainJoinCredentials();

        }

        [CertificateAuth]
        public DtoImpersonationAccount GetImpersonationAccount(string impersonationGuid)
        {
            var enableImpersonationAccess = ConfigurationManager.AppSettings["EnableImpersonationAccess"];
            if (!enableImpersonationAccess.ToLower().Equals("true"))
            {
                Logger.Debug("Retrieving Impersonation Accounts For This Com Server Is Disabled");
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden));
            }
            var clientGuid = RequestContext.Principal.Identity.Name;
            return new ServiceImpersonationAccount().GetForClient(impersonationGuid,clientGuid);

        }

        [ProvisionAuth]
        [HttpPost]
        public DtoApiBoolResponse RemoveDownloadConnection(DtoDownloadConRequest conRequest)
        {
            var client = new ServiceComputer().GetByGuid(conRequest.ComputerGuid);
            if (client == null)
                return new DtoApiBoolResponse() {Value = false};
            var result = new ServiceCurrentDownload().DeleteByClientId(client.Id,conRequest.ComServer);
            return new DtoApiBoolResponse() {Value = result};

        }
    }

    
}
