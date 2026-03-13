using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class AssetGroupAPI : BaseAPI<EntityAssetGroup>
    {
        public AssetGroupAPI(string resource, ProtectedSessionStorage protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

        public bool RemoveGroupMember(int assetGroupId, int assetId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/RemoveGroupMember/{1}", Resource, assetGroupId);
            Request.AddParameter("assetId", assetId);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public IEnumerable<DtoAssetWithType> GetGroupMembers(int assetGroupId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetGroupMembers/{1}", Resource,assetGroupId);
            return _apiRequest.Execute<List<DtoAssetWithType>>(Request);
        }
    }
}