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
    public class ImageProfileSysprepController : ApiController
    {
        private readonly ServiceImageProfileSysprep _imageProfileSysprepService;

        public ImageProfileSysprepController()
        {
            _imageProfileSysprepService = new ServiceImageProfileSysprep();
          
        }

        [Authorize]
        public DtoActionResult Post(EntityImageProfileSysprepTag imageProfileSysprep)
        {
            return _imageProfileSysprepService.AddImageProfileSysprep(imageProfileSysprep);
        }


    }
}