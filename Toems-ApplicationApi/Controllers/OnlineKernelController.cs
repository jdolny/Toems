using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Service;
using Toems_Service.Workflows;

namespace Toems_ApplicationApi.Controllers
{
    public class OnlineKernelController : ApiController
    {
        private readonly ServiceOnlineKernel _onlineKernelServices;

        public OnlineKernelController()
        {
            _onlineKernelServices = new ServiceOnlineKernel();
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public DtoApiBoolResponse DownloadKernel(DtoOnlineKernel onlineKernel)
        {
            return new DtoApiBoolResponse() { Value = new DownloadKernel().RunAllServers(onlineKernel) };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<DtoOnlineKernel> Get()
        {
            return _onlineKernelServices.GetAllOnlineKernels();
        }
    }
}