using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class WinPeModuleAPI : BaseAPI<EntityWinPeModule>
    {

        public WinPeModuleAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {
            
        }

        public string GetArchivedCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public List<EntityWinPeModule> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetArchived", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityWinPeModule>>(Request);
        }
      
       
    }
}