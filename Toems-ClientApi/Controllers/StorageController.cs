using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{

    public class StorageController : ApiController
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse Sync()
        {
            return new DtoApiBoolResponse(){Value = new FolderSync().Sync()};
        }

        [InterComAuth]
        [HttpPost]
        public DtoFreeSpace GetFreeSpace()
        {
            return new Toems_Service.Workflows.ComServerFreeSpace().GetFreeSpace();
        }

        [InterComAuth]
        [HttpPost]
        public List<DtoReplicationProcess> GetReplicationProcesses()
        {
            return new FilesystemServices().GetRunningSyncProcess();
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse KillProcess(int id)
        {
            return new DtoApiBoolResponse() { Value = new FilesystemServices().KillRoboProcess(id) };
        }

        [InterComAuth]
        [HttpPost]
        public HttpResponseMessage GetWinPeDriver(DtoClientFileRequest fileRequest)
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

            var fullPath = Path.Combine(basePath, "software_uploads", fileRequest.ModuleGuid,
                fileRequest.FileName);
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


    }

    
}
