using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class AssetGroupAPI : BaseAPI<EntityAssetGroup>
    {
        public AssetGroupAPI(string resource) : base(resource)
        {

        }

        public bool RemoveGroupMember(int assetGroupId, int assetId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/RemoveGroupMember/{1}", Resource, assetGroupId);
            Request.AddParameter("assetId", assetId);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public IEnumerable<DtoAssetWithType> GetGroupMembers(int assetGroupId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetGroupMembers/{1}", Resource,assetGroupId);
            return new ApiRequest().Execute<List<DtoAssetWithType>>(Request);
        }
    }
}