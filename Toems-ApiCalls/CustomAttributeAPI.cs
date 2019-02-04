using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CustomAttributeAPI : BaseAPI<EntityCustomAttribute>
    {
        public CustomAttributeAPI(string resource) : base(resource)
        {

        }

        public List<EntityCustomAttribute> GetForBuiltInComputers()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetForBuiltInComputers", Resource);
            return new ApiRequest().Execute<List<EntityCustomAttribute>>(Request);
        }

        public List<EntityCustomAttribute> GetForAssetType(int assetTypeId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetForAssetType/{1}", Resource,assetTypeId);
            return new ApiRequest().Execute<List<EntityCustomAttribute>>(Request);
        }

        new public IEnumerable<DtoCustomAttributeWithType> Search(DtoSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(filter);
            Request.Resource = string.Format("{0}/Search", Resource);
            return new ApiRequest().Execute<List<DtoCustomAttributeWithType>>(Request);
        }

    }
}