using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class ImageProfileTemplateController : ApiController
    {
        private readonly ServiceImageProfileTemplate _imageProfileTemplateService;

        public ImageProfileTemplateController()
        {
            _imageProfileTemplateService = new ServiceImageProfileTemplate();
          
        }

        [Authorize]
        public EntityImageProfileTemplate Get(EnumProfileTemplate.TemplateType templateType)
        {
            var result = _imageProfileTemplateService.GetTemplate(templateType);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = "Administrator")]
        public DtoActionResult Put(EntityImageProfileTemplate template)
        {
            var result = _imageProfileTemplateService.UpdateTemplate(template);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}