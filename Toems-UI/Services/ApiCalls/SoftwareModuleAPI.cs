﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class SoftwareModuleAPI : BaseAPI<EntitySoftwareModule>
    {

        public SoftwareModuleAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {
            
        }
        public string GetArchivedCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public List<EntitySoftwareModule> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetArchived", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntitySoftwareModule>>(Request);
        }

        public DtoActionResult GenerateArguments(int moduleId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GenerateArguments/", Resource);
            Request.AddParameter("moduleId", moduleId);
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