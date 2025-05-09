using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ImageProfileAPI : BaseAPI<ImageProfileWithImage>
    {
        public ImageProfileAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public bool Clone(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Clone/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public string GetMinimumClientSize(int id, int hdNumber)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetMinimumClientSize/{1}", Resource, id);
            Request.AddParameter("hdNumber", hdNumber);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public IEnumerable<EntityImageProfileScript> GetScripts(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetScripts/{1}", Resource, id);
            var result = _apiRequest.Execute<List<EntityImageProfileScript>>(Request);
            if (result == null)
                return new List<EntityImageProfileScript>();
            else
                return result;
        }

        public IEnumerable<EntityImageProfileSysprepTag> GetSysprep(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSysprep/{1}", Resource, id);
            var result = _apiRequest.Execute<List<EntityImageProfileSysprepTag>>(Request);
            if (result == null)
                return new List<EntityImageProfileSysprepTag>();
            else
                return result;
        }

        public IEnumerable<EntityImageProfileFileCopy> GetFileCopy(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetFileCopy/{1}", Resource, id);
            var result = _apiRequest.Execute<List<EntityImageProfileFileCopy>>(Request);
            if (result == null)
                return new List<EntityImageProfileFileCopy>();
            else
                return result;
        }

        public bool RemoveProfileFileCopy(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = string.Format("{0}/RemoveProfileFileCopy/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (response != null) return response.Value;
            return false;
        }

        public bool RemoveProfileScripts(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = string.Format("{0}/RemoveProfileScripts/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (response != null) return response.Value;
            return false;
        }

        public bool RemoveProfileSysprepTags(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = string.Format("{0}/RemoveProfileSysprep/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (response != null) return response.Value;
            return false;
        }

        public DtoActionResult Post(EntityImageProfile profile)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(profile), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/Post/",Resource);
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