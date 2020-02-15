using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{

    public class ImagingController : ApiController
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse CopyPxeBinaries()
        {
            return new DtoApiBoolResponse(){Value = new CopyPxeBinaries().Copy()};
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse CreateDefaultBootMenu(DtoBootMenuGenOptions bootOptions)
        {
            return new DtoApiBoolResponse() { Value = new DefaultBootMenu().Create(bootOptions) };
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse DownloadKernel(DtoOnlineKernel onlineKernel)
        {
            return new DtoApiBoolResponse() { Value = new DownloadKernel().Download(onlineKernel) };
        }

        [InterComAuth]
        [HttpPost]
        public List<string> GetKernels()
        {
            return Toems_Service.FilesystemServices.GetKernels();
        }

        [InterComAuth]
        [HttpPost]
        public List<string> GetBootImages()
        {
            return Toems_Service.FilesystemServices.GetBootImages();
        }

       
        [HttpPost]
        public HttpResponseMessage GenerateISO(DtoIsoGenOptions isoOptions)
        {
            var iso = new IsoGenerator().Create(isoOptions);
            var dataStream = new MemoryStream(iso);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(dataStream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "clientboot.iso";
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = dataStream.Length;
            return result;
        }


    }

    
}
