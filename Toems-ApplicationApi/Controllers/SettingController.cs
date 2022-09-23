﻿using System.Collections.Generic;
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

        [HttpGet]
        public DtoApiStringResponse CheckMfaEnabled()
        {
            return new DtoApiStringResponse() { Value = _settingServices.GetSetting(SettingStrings.EnableMfa).Value };
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

        [CustomAuth(Permission = AuthorizationStrings.PxeSettingsUpdate)]
        [HttpPost]
        public DtoApiBoolResponse UpdatePxeSettings(List<EntitySetting> listSettings)
        {
            return new DtoApiBoolResponse { Value = _settingServices.UpdatePxeSetting(listSettings) };
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

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiStringResponse GetMeshAdminPass()
        {
            return new DtoApiStringResponse() { Value = new ServiceSetting().GetMeshUserPass("admin") };
        }

        [CustomAuth(Permission = AuthorizationStrings.RemoteAccessControl)]
        [HttpGet]
        public DtoApiStringResponse GetMeshControlPass()
        {
            return new DtoApiStringResponse() { Value = new ServiceSetting().GetMeshUserPass("control") };
        }

        [CustomAuth(Permission = AuthorizationStrings.RemoteAccessView)]
        [HttpGet]
        public DtoApiStringResponse GetMeshViewPass()
        {
            return new DtoApiStringResponse() { Value = new ServiceSetting().GetMeshUserPass("view") };
        }

        [CustomAuth(Permission = AuthorizationStrings.PxeSettingsUpdate)]
        [HttpGet]
        public DtoApiBoolResponse CopyPxeBinaries()
        {
            return new DtoApiBoolResponse() { Value = new CopyPxeBinaries().RunAllServers() };
        }

        [CustomAuth(Permission = AuthorizationStrings.PxeSettingsUpdate)]
        [HttpPost]
        public DtoApiBoolResponse CreateDefaultBootMenu(DtoBootMenuGenOptions defaultMenuOptions)
        {
            return new DtoApiBoolResponse() { Value = new DefaultBootMenu().RunAllServers(defaultMenuOptions) };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public HttpResponseMessage ExportMsi(bool is64bit)
        {
            var msi = new ServiceMsiUpdater().UpdateMsis(is64bit);
            var fileName = new ServiceMsiUpdater().GetNameForExport(is64bit);
            var dataStream = new MemoryStream(msi);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(dataStream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = dataStream.Length;
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiStringResponse GetMsiFileName(bool is64bit)
        {
            return new DtoApiStringResponse() { Value = new ServiceMsiUpdater().GetNameForExport(is64bit) };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse CopyToecUpgrade()
        {
            return new DtoApiBoolResponse() { Value = _settingServices.CopyMsiToClientUpdate() };
        }



        [CustomAuth(Permission = AuthorizationStrings.PxeISOGen)]
        [HttpPost]
        public HttpResponseMessage GenerateIso(DtoIsoGenOptions isoOptions)
        {
            var iso = new IsoGenerator().RunAllServers(isoOptions);
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