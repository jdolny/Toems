using System.Collections.Generic;
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
    public class GroupCategoryController : ApiController
    {
        private readonly ServiceGroupCategory _groupCategoryServices;

        public GroupCategoryController()
        {
            _groupCategoryServices = new ServiceGroupCategory();
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupUpdate)]
        public DtoActionResult Post(List<EntityGroupCategory> groupCategories)
        {
            return _groupCategoryServices.AddOrUpdate(groupCategories);
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupDelete)]
        public DtoActionResult Delete(int id)
        {
            var result = _groupCategoryServices.DeleteAllForGroup(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


    }
}