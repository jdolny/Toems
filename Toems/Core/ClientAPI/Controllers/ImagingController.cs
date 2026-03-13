using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using log4net;
using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Common.Entity;

using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;

namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class ImagingController(ServiceContext ctx) : ControllerBase
    {
       [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse CopyPxeBinaries()
        {
            return new DtoApiBoolResponse(){Value = ctx.CopyPxeBinaries.Copy()};
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse CheckImageExists(int id)
        {
            return new DtoApiBoolResponse() { Value = ctx.Filessystem.CheckImageExists(id) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse SyncComToSmb(List<int> imageIds)
        {
            return new DtoApiBoolResponse() { Value = ctx.ImageSync.SyncToSmb(imageIds) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse SyncSmbToCom(List<int> imageIds)
        {
            return new DtoApiBoolResponse() { Value = ctx.ImageSync.SyncToCom(imageIds) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse CancelAllImagingTasks()
        {
            return new DtoApiBoolResponse() { Value = ctx.CancelAllImagingTasks.Execute() };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse CleanTaskBootFiles(EntityComputer computer)
        {
            return new DtoApiBoolResponse() { Value = ctx.CleanTaskBootFiles.Execute(computer) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiIntResponse StartUdpSender(DtoMulticastArgs mArgs)
        {
            return new DtoApiIntResponse() { Value = ctx.MulticastArguments.GenerateProcessArguments(mArgs) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse CreateTaskBootFiles(DtoTaskBootFile bootFile)
        {
            return new DtoApiBoolResponse() { Value = ctx.TaskBootMenu.CreatePxeBootFiles(bootFile.Computer,bootFile.ImageProfile) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse KillUdpReceiver(List<int> pids)
        {
            return new DtoApiBoolResponse() { Value = ctx.ActiveImagingTask.KillUdpReceiver(pids) };
        }



        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse TerminateMulticast(EntityActiveMulticastSession multicast)
        {
            return new DtoApiBoolResponse() { Value = ctx.ActiveMulticastSession.TerminateMulticastProcesses(multicast) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse CreateDefaultBootMenu(DtoBootMenuGenOptions bootOptions)
        {
            return new DtoApiBoolResponse() { Value = ctx.DefaultBootMenu.Create(bootOptions) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse DownloadKernel(DtoOnlineKernel onlineKernel)
        {
            return new DtoApiBoolResponse() { Value = ctx.DownloadKernel.Download(onlineKernel) };
        }

        [InterComAuth]
        [HttpPost]
        public List<string> GetComServerLogs()
        {
            return ctx.Filessystem.GetLogs();
        }

        [InterComAuth]
        [HttpPost]
        public List<string> GetComServerLogContents(DtoLogContentRequest logRequest)
        {
            return ctx.Filessystem.GetLogContents(logRequest.name,logRequest.limit);
        }

        [InterComAuth]
        [HttpPost]
        public List<string> GetKernels()
        {
            return ctx.Filessystem.GetKernels();
        }

        [InterComAuth]
        [HttpPost]
        public List<string> GetBootImages()
        {
            return ctx.Filessystem.GetBootImages();
        }

       
        [HttpPost]
        public HttpResponseMessage GenerateISO(DtoIsoGenOptions isoOptions)
        {
            var iso = ctx.IsoGenerator.Create(isoOptions);
            var dataStream = new MemoryStream(iso);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(dataStream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "clientboot.iso";
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = dataStream.Length;
            return result;
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse EditDefaultBootMenu(DtoCoreScript script)
        {
            return new DtoApiBoolResponse
            {
                Value = ctx.Filessystem.EditDefaultBootMenu(script)
            };
        }

        [HttpPost]
        [InterComAuth]
        public DtoApiStringResponse ReadFileText(DtoReadFileText dto)
        {
            return new DtoApiStringResponse
            {
                Value = ctx.Filessystem.ReadAllText(dto.Path)
            };
        }


    }

    
}
