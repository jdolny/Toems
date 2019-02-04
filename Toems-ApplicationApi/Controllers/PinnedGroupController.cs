using System.Net;
using System.Net.Http;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class PinnedGroupController : ApiController
    {
        private readonly ServicePinnedGroup _pinnedGroupService;

        public PinnedGroupController()
        {
            _pinnedGroupService = new ServicePinnedGroup();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public DtoActionResult Delete(int groupId, int userId)
        {
            var result = _pinnedGroupService.Delete(groupId,userId);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public DtoActionResult Post(EntityPinnedGroup pinnedGroup)
        {
            return _pinnedGroupService.Add(pinnedGroup);
        }

       
    }
}