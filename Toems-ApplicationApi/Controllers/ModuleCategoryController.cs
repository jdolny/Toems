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
    public class ModuleCategoryController : ApiController
    {
        private readonly ServiceModuleCategory _moduleCategoryServices;

        public ModuleCategoryController()
        {
            _moduleCategoryServices = new ServiceModuleCategory();
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Post(List<EntityModuleCategory> moduleCategories)
        {
            return _moduleCategoryServices.AddOrUpdate(moduleCategories);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Delete(string moduleGuid)
        {
            var result = _moduleCategoryServices.DeleteAllForModule(moduleGuid);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


    }
}