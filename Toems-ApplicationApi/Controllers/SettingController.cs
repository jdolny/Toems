using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ApplicationApi.Controllers
{
    public class SettingController : ApiController
    {
        private readonly ServiceSetting _settingServices;

        public SettingController()
        {
            _settingServices = new ServiceSetting();
        }

        [HttpGet]
        public string VerifyDb()
        {
            return _settingServices.GetSetting(SettingStrings.CheckinInterval).Value;
        }

        [HttpGet]
        public bool KeepAlive()
        {
            return true;
        }

        [Authorize]
        [HttpGet]
        public DtoApiBoolResponse CheckExpiredToken()
        {
            return new DtoApiBoolResponse { Value = false };
        }

        [Authorize]
        public EntitySetting GetSetting(string name)
        {
            return _settingServices.GetSetting(name);
        }


        public DtoApiStringResponse GetApplicationVersion()
        {
            return new DtoApiStringResponse() { Value = ApplicationStrings.ApplicationVersion };
        }

        [Authorize]
        public DtoApiStringResponse GetSharedLibraryVersion()
        {
            return new DtoApiStringResponse() { Value = SettingStrings.SharedLibraryVersion };
        }


        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse SendEmailTest()
        {
            var mail = new MailServices
            {
                Subject = "Test Message",
                Body = "Email Notifications Are Working!",
                MailTo = ServiceSetting.GetSettingValue(SettingStrings.SmtpMailTo)
            };

            mail.Send();
            return new DtoApiBoolResponse { Value = true };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public DtoApiBoolResponse UpdateSettings(List<EntitySetting> listSettings)
        {
            return new DtoApiBoolResponse { Value = _settingServices.UpdateSetting(listSettings) };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public List<EntityCertificate> GetCAInt()
        {
            return new ServiceCertificate().GetCAIntPair();
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse GenerateCAInt()
        {
            return new DtoApiBoolResponse() { Value = new ServiceCertificate().GenerateCAandInt() };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public HttpResponseMessage ExportCert(int id)
        {
            var cert = new ServiceCertificate().GetCertRawPublic(id);
            var dataStream = new MemoryStream(cert);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(dataStream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "Certificate.cer";
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = dataStream.Length;
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiStringResponse GetClientInstallArgs()
        {
            return new DtoApiStringResponse() { Value = new ServiceSetting().GetClientInstallArgs() };
        }

        [Authorize]
        [HttpGet]
        public DtoApiBoolResponse IsStorageRemote()
        {
            var response = new DtoApiBoolResponse();
            var storageValue = ServiceSetting.GetSettingValue(SettingStrings.StorageType);
            response.Value = storageValue.Equals("SMB");
            return response;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse TestADBind()
        {
            return new DtoApiBoolResponse() { Value = new LdapSync().TestBind() };
        }
    }
}