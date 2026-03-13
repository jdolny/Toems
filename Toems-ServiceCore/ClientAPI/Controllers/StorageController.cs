using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.NoInjectTemp;

namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class StorageController(ServiceContext ctx) : ControllerBase
    {

       [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse Sync()
        {
            return new DtoApiBoolResponse(){Value = ctx.FolderSync.Sync()};
        }

        [InterComAuth]
        [HttpPost]
        public DtoFreeSpace GetFreeSpace()
        {
            return ctx.ComServerFreeSpace.GetFreeSpace();
        }

        [InterComAuth]
        [HttpPost]
        public List<DtoReplicationProcess> GetReplicationProcesses()
        {
            return ctx.Filessystem.GetRunningSyncProcess();
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse KillProcess(int id)
        {
            return new DtoApiBoolResponse() { Value = ctx.Filessystem.KillRoboProcess(id) };
        }

        [InterComAuth]
        [HttpPost]
        public HttpResponseMessage GetWinPeDriver(DtoClientFileRequest fileRequest)
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

            var fullPath = Path.Combine(basePath, "software_uploads", fileRequest.ModuleGuid,
                fileRequest.FileName);
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


    }

    
}
