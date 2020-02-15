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
    public class CustomBootMenuController : ApiController
    {
        private readonly ServiceCustomBootMenu _customBootMenuService;

        public CustomBootMenuController()
        {
            _customBootMenuService = new ServiceCustomBootMenu();
          
        }

         [CustomAuth(Permission = AuthorizationStrings.PxeSettingsUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _customBootMenuService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [Authorize]
        public EntityCustomBootMenu Get(int id)
        {
            var result = _customBootMenuService.GetCustomBootMenu(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [Authorize]
        public IEnumerable<EntityCustomBootMenu> Get()
        {
            return _customBootMenuService.GetAll();
        }

        [Authorize]
        [HttpPost]
        public IEnumerable<EntityCustomBootMenu> Search(DtoSearchFilter filter)
        {
            return _customBootMenuService.Search(filter);
        }

        [Authorize]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _customBootMenuService.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.PxeSettingsUpdate)]
        public DtoActionResult Post(EntityCustomBootMenu customBootMenu)
        {
            return _customBootMenuService.Add(customBootMenu);
        }

        [CustomAuth(Permission = AuthorizationStrings.PxeSettingsUpdate)]
        public DtoActionResult Put(int id, EntityCustomBootMenu customBootMenu)
        {
            customBootMenu.Id = id;
            var result = _customBootMenuService.Update(customBootMenu);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}