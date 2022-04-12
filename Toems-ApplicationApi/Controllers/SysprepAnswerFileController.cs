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
    public class SysprepAnswerFileController : ApiController
    {
        private readonly ServiceSysprepAnswerFile _serviceAnswerFile;
        private readonly int _userId;

        public SysprepAnswerFileController()
        {
           _serviceAnswerFile = new ServiceSysprepAnswerFile();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());

        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var result = _serviceAnswerFile.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        public EntitySysprepAnswerfile Get(int id)
        {
            var result = _serviceAnswerFile.GetAnswerFile(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        public IEnumerable<EntitySysprepAnswerfile> Get()
        {
            return _serviceAnswerFile.GetAll();
        }
     

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntitySysprepAnswerfile answerFile)
        {
            return _serviceAnswerFile.Add(answerFile);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntitySysprepAnswerfile answerFile)
        {
            answerFile.Id = id;
            var result = _serviceAnswerFile.Update(answerFile);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

       


    }
}