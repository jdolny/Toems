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
    public class ToecDeployTargetListController : ApiController
    {
        private readonly ServiceToecDeployTargetList _toecTargetList;

        public ToecDeployTargetListController()
        {
            _toecTargetList = new ServiceToecDeployTargetList();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var result = _toecTargetList.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public EntityToecTargetList Get(int id)
        {
            var result = _toecTargetList.GetToecTargetList(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityToecTargetList> Get()
        {
            return _toecTargetList.Search(new DtoSearchFilter());
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityToecTargetListComputer> GetMembers(int id)
        {
            return _toecTargetList.GetMembers(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public IEnumerable<EntityToecTargetList> Search(DtoSearchFilter filter)
        {
            return _toecTargetList.Search(filter);
        }

         [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _toecTargetList.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntityToecTargetList toecTargetList)
        {
            return _toecTargetList.Add(toecTargetList);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntityToecTargetList toecTargetList)
        {
            toecTargetList.Id = id;
            var result = _toecTargetList.Update(toecTargetList);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}