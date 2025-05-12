using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class AttachmentController : ApiController
    {
        public AttachmentController()
        {

        }
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        
        public HttpResponseMessage GetAttachment(int id, string token)
        {
            var isValidToken = new ServiceBrowserToken().Use(token);
            if (!isValidToken)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            
            var attachment = new ServiceAttachment().Get(id);
            if(attachment == null) return new HttpResponseMessage(HttpStatusCode.NotFound);
            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);

            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    var fullPath = Path.Combine(basePath, "attachments", attachment.DirectoryGuid, attachment.Name);
                    if (File.Exists(fullPath))
                    {
                        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                        try
                        {
                            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                            result.Content = new StreamContent(stream);

                            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            result.Content.Headers.ContentDisposition.FileName = attachment.Name;
                            return result;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message);

                        }
                    }
                }
                else
                {
                    throw new HttpException("Could Not Reach Storage Path");
                }
            }
          
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [CustomAuth(Permission = AuthorizationStrings.AttachmentRead)]
        public EntityAttachment Get(int id)
        {
            return new ServiceAttachment().Get(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.AttachmentAdd)]
        public DtoActionResult Delete(int id)
        {
            var result = new ServiceAttachment().Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}