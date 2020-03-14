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
    public class ImageProfileFileCopyController : ApiController
    {
        private readonly ServiceImageProfileFileCopy _imageProfileFileCopyService;

        public ImageProfileFileCopyController()
        {
            _imageProfileFileCopyService = new ServiceImageProfileFileCopy();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageUpdate)]
        public DtoActionResult Post(EntityImageProfileFileCopy imageProfileFileCopy)
        {
            return _imageProfileFileCopyService.AddImageProfileFileCopy(imageProfileFileCopy);
        }


    }
}