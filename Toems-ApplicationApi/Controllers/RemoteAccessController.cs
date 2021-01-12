using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class RemoteAccessController : ApiController
    {
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse VerifyRemoteAccessInstalled(int id)
        {
            var response = new ServiceRemoteAccess().VerifyRemoteAccessInstalled(id);
            return new DtoApiBoolResponse() { Value = response };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoActionResult InitializeRemotelyServer(int id)
        {
            return new ServiceRemoteAccess().InitializeRemotelyServer(id);
        }

        [Authorize]
        [HttpGet]
        public DtoApiIntResponse GetRemoteAccessCount()
        {
            var result = new ServiceRemoteAccess().GetRemoteAccessServerCount();
            return new DtoApiIntResponse() { Value = result };
        }

        [CustomAuth(Permission = AuthorizationStrings.AllowRemoteControl)]
        [HttpGet]
        public DtoApiStringResponse GetRemoteControlUrl(string id)
        {
            var result = new ServiceRemoteAccess().GetRemoteControlUrl(id);
            return new DtoApiStringResponse() { Value = result };
        }

        [Authorize]
        [HttpPost]
        public DtoApiStringResponse UpdateWebRtc(DtoWebRtc webRtc)
        {
            var result = new ServiceRemoteAccess().UpdateWebRtc(webRtc.DeviceId,webRtc.Mode);
            return new DtoApiStringResponse() { Value = result };
        }

        [Authorize]
        [HttpGet]
        public DtoApiStringResponse IsDeviceOnline(string id)
        {
            var result = new ServiceRemoteAccess().IsDeviceOnline(id);
            return new DtoApiStringResponse() { Value = result };
        }

        [Authorize]
        [HttpGet]
        public DtoApiStringResponse IsWebRtcEnabled(string id)
        {
            var result = new ServiceRemoteAccess().IsWebRtcEnabled(id);
            return new DtoApiStringResponse() { Value = result };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoActionResult CopyRemotelyInstallerToStorage()
        {
            var result = new ServiceRemoteAccess().CopyAgentInstallerToStorage();
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoActionResult HealthCheck()
        {
            var result = new ServiceRemoteAccess().RunHealthCheck();
            return result;
        }

    }
}