using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class UploadedFileAPI : BaseAPI<EntityUploadedFile>
    {

        public UploadedFileAPI(string resource) : base(resource)
        {
            
        }

        public IEnumerable<EntityUploadedFile> GetModuleFiles(string moduleGuid)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetModuleFiles", Resource);
            Request.AddParameter("moduleGuid", moduleGuid);
            return new ApiRequest().Execute<List<EntityUploadedFile>>(Request);
        }
    }
}