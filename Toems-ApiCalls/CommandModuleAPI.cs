﻿using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class CommandModuleAPI : BaseAPI<EntityCommandModule>
    {

        public CommandModuleAPI(string resource) : base(resource)
        {
            
        }

        public string GetArchivedCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public List<EntityCommandModule> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetArchived", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return new ApiRequest().Execute<List<EntityCommandModule>>(Request);
        }

      
       
    }
}