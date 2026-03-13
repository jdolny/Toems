using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ImageCategoryAPI (string resource, ApiRequest apiRequest)
        : BaseAPI<EntityImageCategory>(resource,apiRequest)
    {

        public async Task<DtoActionResult> Post(List<EntityImageCategory> imageCategories)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/Post/";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(imageCategories), ParameterType.RequestBody);
            var response = await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
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