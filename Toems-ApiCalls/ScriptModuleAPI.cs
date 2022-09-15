using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ScriptModuleAPI : BaseAPI<EntityScriptModule>
    {

        public ScriptModuleAPI(string resource) : base(resource)
        {
            
        }

        public string GetArchivedCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public List<EntityScriptModule> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetArchived", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return new ApiRequest().Execute<List<EntityScriptModule>>(Request);

        }

        public List<EntityScriptModule> GetImagingScripts()
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetImagingScripts", Resource);
            return new ApiRequest().Execute<List<EntityScriptModule>>(Request);

        }

        public IEnumerable<EntityScriptModule> GetAllWithInventory()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAllWithInventory", Resource);
            return new ApiRequest().Execute<List<EntityScriptModule>>(Request);
        }

        public IEnumerable<EntityScriptModule> GetConditions()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetConditions", Resource);
            return new ApiRequest().Execute<List<EntityScriptModule>>(Request);
        }


    }
}