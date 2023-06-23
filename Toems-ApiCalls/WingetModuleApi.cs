using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class WingetModuleAPI : BaseAPI<EntityWingetModule>
    {

        public WingetModuleAPI(string resource) : base(resource)
        {
            
        }

        public string GetArchivedCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public EntityWingetLocaleManifest GetLocaleManifest(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetLocaleManifest/{1}", Resource,id);
            return new ApiRequest().Execute<EntityWingetLocaleManifest>(Request);
        }

        public List<EntityWingetModule> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetArchived", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return new ApiRequest().Execute<List<EntityWingetModule>>(Request);
        }

        public List<EntityWingetLocaleManifest> SearchManifests(DtoWingetSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/SearchManifests", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return new ApiRequest().Execute<List<EntityWingetLocaleManifest>>(Request);
        }



    }
}