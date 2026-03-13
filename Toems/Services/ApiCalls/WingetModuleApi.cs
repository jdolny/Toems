using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class WingetModuleAPI : BaseAPI<EntityWingetModule>
    {

        public WingetModuleAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {
            
        }

        public string GetArchivedCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public EntityWingetLocaleManifest GetLocaleManifest(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetLocaleManifest/{1}", Resource,id);
            return _apiRequest.Execute<EntityWingetLocaleManifest>(Request);
        }

        public List<EntityWingetModule> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetArchived", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityWingetModule>>(Request);
        }

        public List<EntityWingetLocaleManifest> SearchManifests(DtoWingetSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/SearchManifests", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityWingetLocaleManifest>>(Request);
        }

        public string GetLastWingetImportTime()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetLastWingetImportTime", Resource);
            var responseData = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }


    }
}