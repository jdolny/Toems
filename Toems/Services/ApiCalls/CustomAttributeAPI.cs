using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

        public class CustomAttributeAPI(string resource, ApiRequest apiRequest)
            : BaseAPI<EntityCustomAttribute>(resource,apiRequest)
        {

        public async Task<List<EntityCustomAttribute>> GetForBuiltInComputers()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetForBuiltInComputers";
            return await _apiRequest.ExecuteAsync<List<EntityCustomAttribute>>(Request);
        }

        public async Task<List<EntityCustomAttribute>> GetForAssetType(int assetTypeId)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetForAssetType/{assetTypeId}";
            return await _apiRequest.ExecuteAsync<List<EntityCustomAttribute>>(Request);
        }

        public new async Task<IEnumerable<DtoCustomAttributeWithType>> Search(DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/Search";
            return await _apiRequest.ExecuteAsync<List<DtoCustomAttributeWithType>>(Request);
        }

    }
}