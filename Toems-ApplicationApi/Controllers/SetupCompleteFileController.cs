using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class SetupCompleteFileController : ApiController
    {
        private readonly ServiceSetupCompleteFile _serviceSetupCompleteFile;
        private readonly int _userId;

        public SetupCompleteFileController()
        {
           _serviceSetupCompleteFile = new ServiceSetupCompleteFile();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());

        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var result = _serviceSetupCompleteFile.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        public EntitySetupCompleteFile Get(int id)
        {
            var result = _serviceSetupCompleteFile.GetSetupCompleteFile(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        public IEnumerable<EntitySetupCompleteFile> Get()
        {
            return _serviceSetupCompleteFile.GetAll();
        }
     

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntitySetupCompleteFile file)
        {
            return _serviceSetupCompleteFile.Add(file);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntitySetupCompleteFile file)
        {
            file.Id = id;
            var result = _serviceSetupCompleteFile.Update(file);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

       


    }
}