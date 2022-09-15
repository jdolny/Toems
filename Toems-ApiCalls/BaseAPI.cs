using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class BaseAPI<T> : IBaseAPI<T> where T : new()
    {

        protected readonly RestRequest Request;
        protected readonly string Resource;

        public BaseAPI(string resource)
        {
            Request = new RestRequest();
            Resource = resource;
        }

        public List<T> Search(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.POST;
            Request.Resource = $"{Resource}/Search";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return new ApiRequest().Execute<List<T>>(Request);
        }

        public List<T> Search(DtoSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.Resource = $"{Resource}/Search";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return new ApiRequest().Execute<List<T>>(Request);
        }

        public List<T> Get()
        {
            Request.Method = Method.GET;
            Request.Resource = $"{Resource}/Get";
            return new ApiRequest().Execute<List<T>>(Request);
        }

        public T Get(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = $"{Resource}/Get/{id}";
            return new ApiRequest().Execute<T>(Request);
        }

        public string GetCount()
        {
            Request.Method = Method.GET;
            Request.Resource = $"{Resource}/GetCount";
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public DtoActionResult Put(int id,T tObject)
        {
            Request.Method = Method.PUT;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(tObject), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/Put/{id}";
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

        public DtoActionResult Post(T tObject)
        {
            Request.Method = Method.POST;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(tObject), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/Post/";
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

        public DtoActionResult Delete(int id)
        {
            Request.Method = Method.DELETE;
            Request.Resource = $"{Resource}/Delete/{id}";
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