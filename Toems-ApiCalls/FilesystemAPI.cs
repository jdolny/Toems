using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class FilesystemAPI
    {
        private readonly RestRequest _request;
        private readonly string _resource;

        public FilesystemAPI(string resource)
        {
            _request = new RestRequest();
            _resource = resource;
        }

        public IEnumerable<string> GetLogContents(string name, int limit)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetLogContents/", _resource);
            _request.AddParameter("name", name);
            _request.AddParameter("limit", limit);
            var response = new ApiRequest().Execute<List<string>>(_request);

            return response;
        }

        public List<string> GetLogs()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetLogs/", _resource);
            var response = new ApiRequest().Execute<List<string>>(_request);

            return response;
        }

     

    

        public string GetServerPaths(string type, string subType)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetServerPaths/", _resource);
            _request.AddParameter("type", type);
            _request.AddParameter("subType", subType);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(_request);

            return response != null ? response.Value : string.Empty;
        }

        public DtoFreeSpace GetFreeSpace(bool isRemote)
        {
            _request.Method = Method.GET;
            _request.AddParameter("isRemote", isRemote);
            _request.Resource = string.Format("{0}/GetFreeSpace/", _resource);
            var response = new ApiRequest().Execute<DtoFreeSpace>(_request);
            return response;
        }

        public bool SyncStorageServers()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/SyncStorageServers/", _resource);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(_request);
            return response != null && response.Value;
        }

  

     
    }
}