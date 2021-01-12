using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class RemoteAccessApi
    {
        private readonly RestRequest _request;
        private readonly string _resource;

        public RemoteAccessApi(string resource)
        {
            _request = new RestRequest();
            _resource = resource;
        }

        public bool VerifyRemoteAccessInstalled(int id)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/VerifyRemoteAccessInstalled/{1}", _resource, id);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(_request);
            return response != null && response.Value;
        }

        public DtoActionResult InitializeRemotelyServer(int id)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/InitializeRemotelyServer/{1}", _resource, id);
            return new ApiRequest().Execute<DtoActionResult>(_request);
        }

        public DtoActionResult HealthCheck()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/HealthCheck/", _resource);
            return new ApiRequest().Execute<DtoActionResult>(_request);
        }

        public DtoActionResult CopyRemotelyInstallerToStorage()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/CopyRemotelyInstallerToStorage/", _resource);
            var result = new ApiRequest().Execute<DtoActionResult>(_request);
            return result;
        }

        public int GetRemoteAccessCount()
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetRemoteAccessCount/", _resource);
            var response = new ApiRequest().Execute<DtoApiIntResponse>(_request);
            if (response == null)
                return -1;
            else
                return response.Value; 
        }

        public string IsDeviceOnline(string deviceId)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/IsDeviceOnline/{1}", _resource, deviceId);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(_request);
            if (response != null) return response.Value;
            else return null;
        }

        public string IsWebRtcEnabled(string deviceId)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/IsWebRtcEnabled/{1}", _resource, deviceId);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(_request);
            if (response != null) return response.Value;
            else return null;
        }

        public string UpdateWebRtc(DtoWebRtc webRtc)
        {
            _request.Method = Method.POST;
            _request.Resource = string.Format("{0}/UpdateWebRtc/", _resource);
            _request.AddJsonBody(webRtc);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(_request);
            if (response != null) return response.Value;
            else return null;
        }

        public string GetRemoteControlUrl(string deviceId)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/GetRemoteControlUrl/{1}", _resource, deviceId);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(_request);
            if (response != null) return response.Value;
            else return null;
        }

        public string CreateRemotelyFirstUser(string remotelyUrl, RemotelyUser user)
        {
            _request.Method = Method.POST;
            _request.AddJsonBody(user);
            _request.Resource = "api/Theopenem/CreateFirstUser";
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(_request,"");
        }

        public string RemotelyUpdateWebRtc(string remotelyUrl, string deviceId, string rtcMode, string auth)
        {
            _request.Method = Method.GET;
            _request.Resource = $"api/Theopenem/UpdateWebRtc/";
            _request.AddParameter("deviceID", deviceId);
            _request.AddParameter("mode", rtcMode);
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(_request, auth);
        }

        public string GetRemoteUrl(string remotelyUrl, string deviceId, string auth)
        {
            _request.Method = Method.GET;
            _request.Resource = $"api/RemoteControl/{deviceId}";
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(_request,auth);
        }

        public string RemotelyIsDeviceOnline(string remotelyUrl, string deviceId, string auth)
        {
            _request.Method = Method.GET;
            _request.Resource = $"api/Theopenem/IsDeviceOnline/";
            _request.AddParameter("deviceID", deviceId);
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(_request, auth);
        }

        public string RemotelyIsWebRtcEnabled(string remotelyUrl, string deviceId, string auth)
        {
            _request.Method = Method.GET;
            _request.Resource = $"api/Theopenem/IsWebRtcEnabled/";
            _request.AddParameter("deviceID", deviceId);
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(_request, auth);
        }

        public string RemotelyStatus(string remotelyUrl)
        {
            _request.Method = Method.GET;
            _request.Resource = $"api/Theopenem/Status/";
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(_request, "");
        }




    }
}
