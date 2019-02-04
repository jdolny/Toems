using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ImpersonationAccountAPI : BaseAPI<EntityImpersonationAccount>
    {
        public ImpersonationAccountAPI(string resource) : base(resource)
        {

        }

        public List<EntityImpersonationAccount> GetForDropDown()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetForDropDown", Resource);
            return new ApiRequest().Execute<List<EntityImpersonationAccount>>(Request);
        }
    }
}