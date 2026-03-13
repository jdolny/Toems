using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class UploadedFileAPI : BaseAPI<EntityUploadedFile>
    {

        public UploadedFileAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {
            
        }

        public IEnumerable<EntityUploadedFile> GetModuleFiles(string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetModuleFiles", Resource);
            Request.AddParameter("moduleGuid", moduleGuid);
            return _apiRequest.Execute<List<EntityUploadedFile>>(Request);
        }
    }
}