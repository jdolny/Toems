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
    public class CategoryController : ApiController
    {
        private readonly ServiceCategory _categoryService;

        public CategoryController()
        {
            _categoryService = new ServiceCategory();
          
        }

         [CustomAuth(Permission = AuthorizationStrings.CategoryDelete)]
        public DtoActionResult Delete(int id)
        {
            var result = _categoryService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [Authorize]
        public EntityCategory Get(int id)
        {
            var result = _categoryService.GetCategory(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [Authorize]
        public IEnumerable<EntityCategory> Get()
        {
            return _categoryService.GetAll();
        }

        [Authorize]
        [HttpPost]
        public IEnumerable<EntityCategory> Search(DtoSearchFilter filter)
        {
            return _categoryService.Search(filter);
        }

        [Authorize]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _categoryService.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.CategoryUpdate)]
        public DtoActionResult Post(EntityCategory category)
        {
            return _categoryService.Add(category);
        }

        [CustomAuth(Permission = AuthorizationStrings.CategoryUpdate)]
        public DtoActionResult Put(int id, EntityCategory category)
        {
            category.Id = id;
            var result = _categoryService.Update(category);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}