using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ModuleCategoryAPI : BaseAPI<EntityModuleCategory>
    {

        public ModuleCategoryAPI(string resource) : base(resource)
        {
            
        }

        public DtoActionResult Post(List<EntityModuleCategory> moduleCategories)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddJsonBody(moduleCategories);
            var response = new ApiRequest().Execute<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public DtoActionResult Delete(string moduleGuid)
        {
            Request.Method = Method.DELETE;
            Request.Resource = $"{Resource}/Delete/";
            Request.AddParameter("moduleGuid", moduleGuid);
            var response = new ApiRequest().Execute<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }
    }
}