
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class BaseAPI<T> : IBaseAPI<T> where T : new()
    {

        protected readonly string Resource;
        protected RestRequest Request { get; }
        protected readonly ApiRequest _apiRequest;

        protected BaseAPI(string resource, ApiRequest apiRequest)
        {
            Resource = resource;
            Request = new RestRequest();
            _apiRequest = apiRequest;
        }

        public async Task<List<T>> Search(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/Search";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<List<T>>(Request);
        }

        public async Task<List<T>> Search(DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/Search";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<List<T>>(Request);
        }

        public async Task<List<T>> Get()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/Get";
            return await _apiRequest.ExecuteAsync<List<T>>(Request);
        }

        public async Task<T> Get(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/Get/{id}";
            return await _apiRequest.ExecuteAsync<T>(Request);
        }

        public async Task<string> GetCount()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetCount";
            var responseData = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public async Task<DtoActionResult> Put(int id,T tObject)
        {
            Request.Method = Method.Put;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(tObject), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/Put/{id}";
            var response = await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
            if (response.Id == 0)
                response.Success = false;
            return response;
        }

        public async Task<DtoActionResult> Post(T tObject)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(tObject), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/Post/";
            var response = await _apiRequest.ExecuteAsync<DtoActionResult?>(Request);
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

        public async Task<DtoActionResult> Delete(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = $"{Resource}/Delete/{id}";
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