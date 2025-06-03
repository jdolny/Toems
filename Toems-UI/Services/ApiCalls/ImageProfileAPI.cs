using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ImageProfileAPI (string resource, ApiRequest apiRequest)
        : BaseAPI<ImageProfileWithImage>(resource,apiRequest)
    {

        public async Task<bool> Clone(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/Clone/{id}";
            var response = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public async Task<string> GetMinimumClientSize(int id, int hdNumber)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetMinimumClientSize/{id}";
            Request.AddParameter("hdNumber", hdNumber);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public async Task<IEnumerable<EntityImageProfileScript>> GetScripts(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetScripts/{id}";
            var result = await _apiRequest.ExecuteAsync<List<EntityImageProfileScript>>(Request);
            if (result == null)
                return new List<EntityImageProfileScript>();
            else
                return result;
        }

        public async Task<IEnumerable<EntityImageProfileSysprepTag>> GetSysprep(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetSysprep/{id}";
            var result = await _apiRequest.ExecuteAsync<List<EntityImageProfileSysprepTag>>(Request);
            if (result == null)
                return new List<EntityImageProfileSysprepTag>();
            else
                return result;
        }

        public async Task<IEnumerable<EntityImageProfileFileCopy>> GetFileCopy(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetFileCopy/{id}";
            var result = await _apiRequest.ExecuteAsync<List<EntityImageProfileFileCopy>>(Request);
            if (result == null)
                return new List<EntityImageProfileFileCopy>();
            else
                return result;
        }

        public async Task<bool> RemoveProfileFileCopy(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = $"{Resource}/RemoveProfileFileCopy/{id}";
            var response = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (response != null) return response.Value;
            return false;
        }

        public async Task<bool> RemoveProfileScripts(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = $"{Resource}/RemoveProfileScripts/{id}";
            var response = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (response != null) return response.Value;
            return false;
        }

        public async Task<bool> RemoveProfileSysprepTags(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = $"{Resource}/RemoveProfileSysprep/{id}";
            var response = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (response != null) return response.Value;
            return false;
        }

        public async Task<DtoActionResult> Post(EntityImageProfile profile)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(profile), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/Post/";
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