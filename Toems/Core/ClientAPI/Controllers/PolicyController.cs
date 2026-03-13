using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.NoInjectTemp;
using Toems_ServiceCore.Workflows;


namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class PolicyController(ServiceContext ctx) : ControllerBase
    {
        
        [CertificateAuth]
        [HttpPost]
        public DtoTriggerResponse GetClientPolicies(DtoPolicyRequest policyRequest)
        {
            return ctx.GetClientPolicies.Execute(policyRequest);
        }

        [CertificateAuth]
        [HttpPost]
        public DtoActionResult AddHistory(DtoPolicyResults results)
        {
            var clientGuid = HttpContext.User.Identity?.Name;
            return ctx.PolicyHistory.AddHistories(results, clientGuid);

        }

        [CertificateAuth]
        [HttpPost]
        public DtoActionResult UpdateLastSocketResult(DtoApiStringResponse result)
        {
            var clientGuid = HttpContext.User.Identity?.Name;
            return ctx.Computer.UpdateSocketResult(result.Value, clientGuid);

        }

        [CertificateAuth]
        [HttpGet]
        public List<EntityWingetModule> GetComputerWingetModules()
        {
            var clientGuid = HttpContext.User.Identity?.Name;
            return ctx.Computer.GetComputerWingetUpgrades(clientGuid);

        }

        [CertificateAuth]
        [HttpGet]
        public DtoApiStringResponse GetRemotelyInstallArgs()
        {
            return new DtoApiStringResponse() { Value = ctx.RemoteAccess.GetRemotelyInstallArgs() };
        }


        [CertificateAuth]
        [HttpPost]
        public DtoActionResult UpdateRemoteAccessId(DtoRemotelyConnectionInfo conInfo)
        {
            var clientGuid = HttpContext.User.Identity?.Name;
            return ctx.RemoteAccess.UpdateComputerRemoteAccessId(conInfo, clientGuid);
        }

        [ProvisionAuth]
        [HttpPost]
        public DtoDownloadConnectionResult CreateDownloadConnection(DtoDownloadConRequest conRequest)
        {
            var result = new DtoDownloadConnectionResult();

            var guid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
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
           
            var client = ctx.Computer.GetByGuid(conRequest.ComputerGuid);
            if (client == null)
            {
                result.ErrorMessage = "Could Not Find Computer With Guid: " + HttpContext.User.Identity?.Name;
                return result;
            }

            
            var currentConnections = ctx.CurrentDownload.TotalCount(conRequest.ComServer);
            var activeClient = ctx.CurrentDownload.GetByClientId(client.Id,conRequest.ComServer);
            if (activeClient == null && (currentConnections >= maxClientConnections))
            {
                var activeCountAfterExpire = ctx.CurrentDownload.ExpireOldConnections();
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
                ctx.CurrentDownload.Add(currentDownload);
                result.Success = true;
                result.QueueIsFull = false;
            }
            //update time for existing download connection
            else
            {
                activeClient.LastRequestTimeLocal = DateTime.Now;
                ctx.CurrentDownload.Update(activeClient);
                result.Success = true;
                result.QueueIsFull = false;
            }

            return result;
        }

        [CertificateAuth]
        [HttpPost]
        public HttpResponseMessage GetFile(DtoClientFileRequest fileRequest)
        {
            var guid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(guid);
            if(thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            var maxBitRate = thisComServer.EmMaxBps;
            var basePath = thisComServer.LocalStoragePath;

            var fullPath = Path.Combine(basePath, "software_uploads", fileRequest.ModuleGuid,
                fileRequest.FileName);
            if (System.IO.File.Exists(fullPath))
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
                    ctx.Log.Error(ex.Message);

                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [ProvisionAuth]
        [HttpPost]
        public HttpResponseMessage GetClientMsi(DtoClientFileRequest fileRequest)
        {
            var guid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            var maxBitRate = thisComServer.EmMaxBps;
            var basePath = thisComServer.LocalStoragePath;

            var fullPath = Path.Combine(basePath, "client_versions", fileRequest.FileName);
            if (System.IO.File.Exists(fullPath))
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
                    ctx.Log.Error(ex.Message);

                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [CertificateAuth]
        public DtoApiStringResponse GetScript(string moduleGuid)
        {
            var script = ctx.ScriptModule.GetModuleByGuid(moduleGuid);
            return new DtoApiStringResponse() { Value = script.ScriptContents };

        }

        [CertificateAuth]
        [HttpGet]
        public DtoApiBoolResponse AddToFirstRunGroup()
        {
            var clientGuid = HttpContext.User.Identity?.Name;
            var client = ctx.Computer.GetByGuid(clientGuid);
            if (client == null)
                return new DtoApiBoolResponse() { Value = false };
            var membership = new EntityGroupMembership();
            membership.ComputerId = client.Id;
            membership.GroupId = -2;

            ctx.GroupMembership.AddMembership(new List<EntityGroupMembership>() { membership });
            return new DtoApiBoolResponse() { Value = true };
        }

        [CertificateAuth]
        [HttpGet]
        public DtoApiBoolResponse RemoveFromFirstRunGroup()
        {
            var clientGuid = HttpContext.User.Identity?.Name;
            var client = ctx.Computer.GetByGuid(clientGuid);
            if (client == null)
                return new DtoApiBoolResponse() { Value = false };

            ctx.Group.DeleteMembership(client.Id, -2);
            return new DtoApiBoolResponse() { Value = true };
        }

        [CertificateAuth]
        public ActionResult<DtoDomainJoinCredentials> GetDomainJoinCredentials()
        {
            var enableImpersonationAccess = ctx.Config["EnableImpersonationAccess"];
            if (!enableImpersonationAccess.ToLower().Equals("true"))
            {
                ctx.Log.Debug("Retrieving Domain Credentials For This Com Server Is Disabled");
                return Forbid();
            }
            return ctx.Setting.GetDomainJoinCredentials();
        }

        [CertificateAuth]
        public ActionResult<DtoImpersonationAccount> GetImpersonationAccount(string impersonationGuid)
        {
            var enableImpersonationAccess = ctx.Config["EnableImpersonationAccess"];
            if (!enableImpersonationAccess.ToLower().Equals("true"))
            {
                ctx.Log.Debug("Retrieving Impersonation Accounts For This Com Server Is Disabled");
                return Forbid();
            }
            var clientGuid = HttpContext.User.Identity?.Name;
            return ctx.ImpersonationAccount.GetForClient(impersonationGuid,clientGuid);

        }

        [ProvisionAuth]
        [HttpPost]
        public DtoApiBoolResponse RemoveDownloadConnection(DtoDownloadConRequest conRequest)
        {
            var client = ctx.Computer.GetByGuid(conRequest.ComputerGuid);
            if (client == null)
                return new DtoApiBoolResponse() {Value = false};
            var result = ctx.CurrentDownload.DeleteByClientId(client.Id,conRequest.ComServer);
            return new DtoApiBoolResponse() {Value = result};

        }
    }

    
}
