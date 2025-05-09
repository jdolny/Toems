using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class RemoteAccessApi : BaseAPI<DtoPlaceHolder>
    {

        public RemoteAccessApi(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public bool VerifyRemoteAccessInstalled(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/VerifyRemoteAccessInstalled/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public DtoActionResult InitializeRemotelyServer(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/InitializeRemotelyServer/{1}", Resource, id);
            return _apiRequest.Execute<DtoActionResult>(Request);
        }

        public DtoActionResult HealthCheck()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/HealthCheck/", Resource);
            return _apiRequest.Execute<DtoActionResult>(Request);
        }

        public DtoActionResult CopyRemotelyInstallerToStorage()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CopyRemotelyInstallerToStorage/", Resource);
            var result = _apiRequest.Execute<DtoActionResult>(Request);
            return result;
        }

        public int GetRemoteAccessCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetRemoteAccessCount/", Resource);
            var response = _apiRequest.Execute<DtoApiIntResponse>(Request);
            if (response == null)
                return -1;
            else
                return response.Value; 
        }

        public string IsDeviceOnline(string deviceId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/IsDeviceOnline/{1}", Resource, deviceId);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null) return response.Value;
            else return null;
        }

        public string IsWebRtcEnabled(string deviceId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/IsWebRtcEnabled/{1}", Resource, deviceId);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null) return response.Value;
            else return null;
        }

        public string UpdateWebRtc(DtoWebRtc webRtc)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/UpdateWebRtc/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(webRtc), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null) return response.Value;
            else return null;
        }

        public string GetRemoteControlUrl(string deviceId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetRemoteControlUrl/{1}", Resource, deviceId);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null) return response.Value;
            else return null;
        }

        public string CreateRemotelyFirstUser(string remotelyUrl, RemotelyUser user)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(user), ParameterType.RequestBody);
            Request.Resource = "api/Theopenem/CreateFirstUser";
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(Request,"");
        }

        public string RemotelyUpdateWebRtc(string remotelyUrl, string deviceId, string rtcMode, string auth)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/Theopenem/UpdateWebRtc/";
            Request.AddParameter("deviceID", deviceId);
            Request.AddParameter("mode", rtcMode);
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(Request, auth);
        }

        public string GetRemoteUrl(string remotelyUrl, string deviceId, string auth)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/RemoteControl/{deviceId}";
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(Request,auth);
        }

        public string RemotelyIsDeviceOnline(string remotelyUrl, string deviceId, string auth)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/Theopenem/IsDeviceOnline/";
            Request.AddParameter("deviceID", deviceId);
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(Request, auth);
        }

        public string RemotelyIsWebRtcEnabled(string remotelyUrl, string deviceId, string auth)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/Theopenem/IsWebRtcEnabled/";
            Request.AddParameter("deviceID", deviceId);
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(Request, auth);
        }

        public string RemotelyStatus(string remotelyUrl)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/Theopenem/Status/";
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(Request, "");
        }




    }
}
