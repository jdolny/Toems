using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class FileCopyModuleAPI : BaseAPI<EntityFileCopyModule>
    {

        public FileCopyModuleAPI(string resource) : base(resource)
        {
            
        }

        public string GetArchivedCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public List<EntityFileCopyModule> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetArchived", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return new ApiRequest().Execute<List<EntityFileCopyModule>>(Request);
        }

        public List<EntityFileCopyModule> GetDriverList()
        {
            Request.Resource = string.Format("{0}/GetDriverList", Resource);
            return new ApiRequest().Execute<List<EntityFileCopyModule>>(Request);
        }


    }
}