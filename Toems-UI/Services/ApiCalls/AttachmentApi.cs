using Blazored.LocalStorage;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class AttachmentAPI : BaseAPI<EntityAttachment>
    {
        public AttachmentAPI(string resource, ILocalStorageService protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

        public async  Task<byte[]> GetAttachment(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetAttachment/{id}";
            return await _apiRequest.ExecuteRawAsync(Request);
        }

        

        
    }
}