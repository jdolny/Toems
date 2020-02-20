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
    public class ImageProfileScriptController : ApiController
    {
        private readonly ServiceImageProfileScript _imageProfileScriptService;

        public ImageProfileScriptController()
        {
            _imageProfileScriptService = new ServiceImageProfileScript();
          
        }

        [Authorize]
        public DtoActionResult Post(EntityImageProfileScript imageProfileScript)
        {
            return _imageProfileScriptService.AddImageProfileScript(imageProfileScript);
        }


    }
}