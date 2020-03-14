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
    public class ImageCategoryController : ApiController
    {
        private readonly ServiceImageCategory _imageCategoryServices;

        public ImageCategoryController()
        {
            _imageCategoryServices = new ServiceImageCategory();
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageUpdate)]
        public DtoActionResult Post(List<EntityImageCategory> imageCategories)
        {
            return _imageCategoryServices.AddOrUpdate(imageCategories);
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _imageCategoryServices.DeleteAllForImage(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


    }
}