﻿using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class AssetGroupMemberAPI : BaseAPI<EntityAssetGroupMember>
    {

        public AssetGroupMemberAPI(string resource) : base(resource)
        {
            
        }

        public DtoActionResult Post(List<EntityAssetGroupMember> assetGroupMembers)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(assetGroupMembers), ParameterType.RequestBody);
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