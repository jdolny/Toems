using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class RemoteAccessApi (string resource, ApiRequest apiRequest)
        : BaseAPI<DtoRemoteAccessFake>(resource,apiRequest)
    {

        public async Task<bool> VerifyRemoteAccessInstalled(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/VerifyRemoteAccessInstalled/{id}";
            var response = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public async Task<DtoActionResult> InitializeRemotelyServer(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/InitializeRemotelyServer/{id}";
            return await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
        }

        public async Task<DtoActionResult> HealthCheck()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/HealthCheck/";
            return await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
        }

        public async Task<DtoActionResult> CopyRemotelyInstallerToStorage()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/CopyRemotelyInstallerToStorage/";
            var result = await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
            return result;
        }

        public async Task<int> GetRemoteAccessCount()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetRemoteAccessCount/";
            var response = await _apiRequest.ExecuteAsync<DtoApiIntResponse>(Request);
            if (response == null)
                return -1;
            else
                return response.Value; 
        }

        public async Task<string> IsDeviceOnline(string deviceId)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/IsDeviceOnline/{deviceId}";
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (response != null) return response.Value;
            else return null;
        }

        public async Task<string> IsWebRtcEnabled(string deviceId)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/IsWebRtcEnabled/{deviceId}";
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (response != null) return response.Value;
            else return null;
        }

        public async Task<string> UpdateWebRtc(DtoWebRtc webRtc)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/UpdateWebRtc/";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(webRtc), ParameterType.RequestBody);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (response != null) return response.Value;
            else return null;
        }

        public async Task<string> GetRemoteControlUrl(string deviceId)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetRemoteControlUrl/{deviceId}";
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (response != null) return response.Value;
            else return null;
        }

        public async Task<string> CreateRemotelyFirstUser(string remotelyUrl, RemotelyUser user)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(user), ParameterType.RequestBody);
            Request.Resource = "api/Theopenem/CreateFirstUser";
            return await new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotelyAsync(Request,"");
        }

        public async Task<string> RemotelyUpdateWebRtc(string remotelyUrl, string deviceId, string rtcMode, string auth)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/Theopenem/UpdateWebRtc/";
            Request.AddParameter("deviceID", deviceId);
            Request.AddParameter("mode", rtcMode);
            return await new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotelyAsync(Request, auth);
        }

        public async Task<string> GetRemoteUrl(string remotelyUrl, string deviceId, string auth)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/RemoteControl/{deviceId}";
            return await new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotelyAsync(Request,auth);
        }

        public async Task<string> RemotelyIsDeviceOnline(string remotelyUrl, string deviceId, string auth)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/Theopenem/IsDeviceOnline/";
            Request.AddParameter("deviceID", deviceId);
            return await new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotelyAsync(Request, auth);
        }

        public async Task<string> RemotelyIsWebRtcEnabled(string remotelyUrl, string deviceId, string auth)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/Theopenem/IsWebRtcEnabled/";
            Request.AddParameter("deviceID", deviceId);
            return await new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotelyAsync(Request, auth);
        }

        public async Task<string> RemotelyStatus(string remotelyUrl)
        {
            Request.Method = Method.Get;
            Request.Resource = $"api/Theopenem/Status/";
            return await new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotelyAsync(Request, "");
        }




    }
    public class DtoRemoteAccessFake
    {}
}
