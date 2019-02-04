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
    public class ImpersonationAccountController : ApiController
    {
        private readonly ServiceImpersonationAccount _impersonationAccountService;

        public ImpersonationAccountController()
        {
            _impersonationAccountService = new ServiceImpersonationAccount();
          
        }

         [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var result = _impersonationAccountService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public EntityImpersonationAccount Get(int id)
        {
            var result = _impersonationAccountService.GetAccount(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityImpersonationAccount> Get()
        {
            return _impersonationAccountService.GetAll();
        }

        [Authorize]
        public IEnumerable<EntityImpersonationAccount> GetForDropDown()
        {
            return _impersonationAccountService.GetForDropDown();
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public IEnumerable<EntityImpersonationAccount> Search(DtoSearchFilter filter)
        {
            return _impersonationAccountService.Search(filter);
        }

         [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _impersonationAccountService.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntityImpersonationAccount user)
        {
            return _impersonationAccountService.Add(user);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntityImpersonationAccount account)
        {
            account.Id = id;
            var result = _impersonationAccountService.Update(account);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}