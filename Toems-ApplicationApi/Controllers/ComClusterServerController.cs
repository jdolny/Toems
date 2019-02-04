using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class ComClusterServerController : ApiController
    {
        private readonly ServiceComClusterServer _comClusterServerService;

        public ComClusterServerController()
        {
            _comClusterServerService = new ServiceComClusterServer();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(List<EntityComServerClusterServer> listOfServers)
        {
            return _comClusterServerService.AddList(listOfServers);
        }
    }
}