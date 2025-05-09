using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class FilesystemAPI : BaseAPI<DtoPlaceHolder>
    {

        public FilesystemAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public IEnumerable<string> GetLogContents(string name, int limit)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetLogContents/", Resource);
            Request.AddParameter("name", name);
            Request.AddParameter("limit", limit);
            var response = _apiRequest.Execute<List<string>>(Request);

            return response;
        }

        public IEnumerable<string> GetComServerLogContents(string name, int limit, int comServerId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComServerLogContents/", Resource);
            Request.AddParameter("name", name);
            Request.AddParameter("limit", limit);
            Request.AddParameter("comServerId", comServerId);
            var response = _apiRequest.Execute<List<string>>(Request);

            return response;
        }

        public List<string> GetLogs()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetLogs/", Resource);
            var response = _apiRequest.Execute<List<string>>(Request);

            return response;
        }

        public List<string> GetComServerLogs(int comServerId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComServerLogs/{1}", Resource,comServerId);
            var response = _apiRequest.Execute<List<string>>(Request);

            return response;
        }

        public List<string> GetKernels()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetKernels/", Resource);
            var response = _apiRequest.Execute<List<string>>(Request);

            return response;
        }


        public List<string> GetBootImages()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetBootImages/", Resource);
            var response = _apiRequest.Execute<List<string>>(Request);

            return response;
        }






        public DtoFreeSpace GetSMBFreeSpace()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSMBFreeSpace/", Resource);
            var response = _apiRequest.Execute<DtoFreeSpace>(Request);
            return response;
        }

        public List<DtoFreeSpace> GetComFreeSpace()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComFreeSpace/", Resource);
            var response = _apiRequest.Execute<List<DtoFreeSpace>>(Request);
            return response;
        }

        public bool SyncStorageServers()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/SyncStorageServers/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

   




    }
}