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

        public IEnumerable<string> GetComServerLogContents(string name, int limit, int comServerId)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetComServerLogContents/", _resource);
            _request.AddParameter("name", name);
            _request.AddParameter("limit", limit);
            _request.AddParameter("comServerId", comServerId);
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

        public List<string> GetComServerLogs(int comServerId)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetComServerLogs/{1}", _resource,comServerId);
            var response = new ApiRequest().Execute<List<string>>(_request);

            return response;
        }

        public List<string> GetKernels()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetKernels/", _resource);
            var response = new ApiRequest().Execute<List<string>>(_request);

            return response;
        }


        public List<string> GetBootImages()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetBootImages/", _resource);
            var response = new ApiRequest().Execute<List<string>>(_request);

            return response;
        }






        public DtoFreeSpace GetSMBFreeSpace()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetSMBFreeSpace/", _resource);
            var response = new ApiRequest().Execute<DtoFreeSpace>(_request);
            return response;
        }

        public List<DtoFreeSpace> GetComFreeSpace()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetComFreeSpace/", _resource);
            var response = new ApiRequest().Execute<List<DtoFreeSpace>>(_request);
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