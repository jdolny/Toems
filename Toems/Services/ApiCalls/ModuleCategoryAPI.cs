using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ModuleCategoryAPI : BaseAPI<EntityModuleCategory>
    {

        public ModuleCategoryAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {
            
        }

        public DtoActionResult Post(List<EntityModuleCategory> moduleCategories)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(moduleCategories), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
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
            Request.Method = Method.Delete;
            Request.Resource = $"{Resource}/Delete/";
            Request.AddParameter("moduleGuid", moduleGuid);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
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