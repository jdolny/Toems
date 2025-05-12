using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Blazored.LocalStorage;
using log4net;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RestSharp;
using Microsoft.AspNetCore.Http;

namespace Toems_ApiCalls
{
    public class ApiRequest
    {
        private readonly RestClient _client;
        private readonly ILog _log = LogManager.GetLogger(typeof(ApiRequest));
        private readonly ILocalStorageService _protectedSessionStorage;

        public ApiRequest(ILocalStorageService protectedSessionStorage)
        {
            var options = new RestClientOptions
            {
                BaseUrl = new Uri("http://localhost:8080/"),
                Timeout = TimeSpan.FromHours(1)
            };
            _client = new RestClient(options);
            _protectedSessionStorage = protectedSessionStorage;
        }

        public ApiRequest(Uri baseUrl)
        {
            var options = new RestClientOptions
            {
                BaseUrl = baseUrl,
                Timeout = TimeSpan.FromHours(1)
            };
            _client = new RestClient(options);
        }

        public TClass ExecuteHMAC<TClass>(RestRequest request, X509Certificate2 cert) where TClass : new()
        {
            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            string nonce = Guid.NewGuid().ToString("N");

            var url =
                System.Web.HttpUtility.UrlEncode(_client.Options.BaseUrl + request.Resource.ToLower());

            var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            string requestContentBase64String = string.Empty;
            if (body != null)
            {
                byte[] content = Encoding.ASCII.GetBytes(body.Value.ToString());
                MD5 md5 = MD5.Create();
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            string signatureRawData = String.Format("{0}{1}{2}{3}{4}", request.Method, url, requestTimeStamp, nonce, requestContentBase64String);
            var csp = (RSACryptoServiceProvider)cert.PrivateKey;
            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(signatureRawData);
            byte[] hash = sha1.ComputeHash(data);
            var signature = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
         
            request.AddHeader("Authorization", "amx " + string.Format("{0}:{1}:{2}", Convert.ToBase64String(signature), nonce, requestTimeStamp));

            var response = _client.Execute<TClass>(request);

            if (response == null)
            {
                _log.Error("Could Not Complete API Request.  The Response was empty." + request.Resource);
                return default(TClass);
            }

            if (response.Data == null)
            {
                _log.Error("Response Data Was Null For Resource: " + request.Resource);
                return default(TClass);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                _log.Error("Could Not Complete API Request.  The Response Produced An Error." + request.Resource);
                _log.Error(response.Content);
                return default(TClass);
            }

            /*if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpException();
            }*/

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _log.Error("Error Retrieving API Response: Not Found " + request.Resource);
               // throw new HttpException();
            }

            if (response.ErrorException != null)
            {
                _log.Error("Error Retrieving API Response: " + response.ErrorException);
                return default(TClass);
            }

            _log.Debug(request.Resource);
            return response.Data;
        }

        public bool DownloadWinPeDriver(RestRequest request, string destination, string serverName, string interComKey)
        {
            //Calculate UNIX time
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            var nonce = Guid.NewGuid().ToString("N");

            var url =
                HttpUtility.UrlEncode(_client.Options.BaseUrl + request.Resource.ToLower());

            var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            var requestContentBase64String = string.Empty;
            if (body != null)
            {
                var content = Encoding.ASCII.GetBytes(body.Value.ToString());
                var md5 = MD5.Create();
                var requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            var signatureRawData = string.Format("{0}{1}{2}{3}{4}{5}", serverName, request.Method, url,
                requestTimeStamp, nonce, requestContentBase64String);


            var signature = Encoding.UTF8.GetBytes(signatureRawData);
            string requestSignatureBase64String;
            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(interComKey)))
            {
                var signatureBytes = hmac.ComputeHash(signature);
                requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
            }

            request.AddHeader("Authorization",
                "amx " +
                string.Format("{0}:{1}:{2}:{3}", serverName, requestSignatureBase64String, nonce, requestTimeStamp));

            try
            {
                _log.Debug(request.Resource);
                using (var stream = System.IO.File.Create(destination, 4096))
                {
                    //request.ResponseWriter = (responseStream) => responseStream.CopyTo(stream);
                    _client.DownloadData(request);
                    if (stream.Length == 0)
                    {
                        //something went wrong, rest sharp can't display any other info with downloaddata, so we don't know why
                        return false;

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Could Not Save File: " + destination);
                _log.Error(ex.Message);
                return false;
            }

        }

        public TClass ExecuteHMACInterCom<TClass>(RestRequest request, string serverName, string interComKey) where TClass : new()
        {
            //Calculate UNIX time
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            var nonce = Guid.NewGuid().ToString("N");

            var url =
                HttpUtility.UrlEncode(_client.Options.BaseUrl + request.Resource.ToLower());

            var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            var requestContentBase64String = string.Empty;
            if (body != null)
            {
                var content = Encoding.ASCII.GetBytes(body.Value.ToString());
                var md5 = MD5.Create();
                var requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            var signatureRawData = string.Format("{0}{1}{2}{3}{4}{5}", serverName, request.Method, url,
                requestTimeStamp, nonce, requestContentBase64String);


            var signature = Encoding.UTF8.GetBytes(signatureRawData);
            string requestSignatureBase64String;
            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(interComKey)))
            {
                var signatureBytes = hmac.ComputeHash(signature);
                requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
            }

            request.AddHeader("Authorization",
                "amx " +
                string.Format("{0}:{1}:{2}:{3}", serverName, requestSignatureBase64String, nonce, requestTimeStamp));

            var response = _client.Execute<TClass>(request);

            if (response == null)
            {
                _log.Error("Could Not Complete API Request.  The Response was empty." + request.Resource);
                return default(TClass);
            }

            if (response.Data == null)
            {
                _log.Error("Response Data Was Null For Resource: " + request.Resource);
                return default(TClass);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                _log.Error("Could Not Complete API Request.  The Response Produced An Error." + request.Resource);
                _log.Error(response.Content);
                return default(TClass);
            }

            /*if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpException();
            }*/

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _log.Error("Error Retrieving API Response: Not Found " + request.Resource);
                //throw new HttpException();
            }

            if (response.ErrorException != null)
            {
                _log.Error("Error Retrieving API Response: " + response.ErrorException);
                return default(TClass);
            }

            _log.Debug(request.Resource);
            return response.Data;
        }

        public TClass Execute<TClass>(RestRequest request) where TClass : new()
        {
            if (request == null)
            {
                _log.Error("Could Not Execute API Request.  The Request was empty." + new TClass().GetType());
                return default(TClass);
            }
            var token = Task.Run(() => GetTokenAsync()).Result;
            if(token != null)
                request.AddHeader("Authorization", "bearer " + token);

            var response = _client.Execute<TClass>(request);

            if (response == null)
            {
                _log.Error("Could Not Complete API Request.  The Response was empty." + request.Resource);
                return default(TClass);
            }

            if (response.Data == null)
            {
                _log.Error("Response Data Was Null For Resource: " + request.Resource);
                return default(TClass);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                _log.Error("Could Not Complete API Request.  The Response Produced An Error." + request.Resource);
                _log.Error(response.Content);
                return default(TClass);
            }

            /*if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpException();
            }*/

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _log.Error("Error Retrieving API Response: Not Found " + request.Resource);
                //throw new HttpException();
            }

            if (response.ErrorException != null)
            {
                _log.Error("Error Retrieving API Response: " + response.ErrorException);
                return default(TClass);
            }

            _log.Debug(request.Resource);
            return response.Data;
        }

        public async Task<TClass> ExecuteAsync<TClass>(RestRequest request) where TClass : new()
        {
            var token = await GetTokenAsync();
            request.AddHeader("Authorization", "bearer " + token);

            var response = await _client.ExecuteAsync<TClass>(request);
            

            if (response.Data == null)
            {
                _log.Error("Response Data Was Null For Resource: " + request.Resource);
                return default(TClass);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                _log.Error("Could Not Complete API Request.  The Response Produced An Error." + request.Resource);
                _log.Error(response.Content);
                return default(TClass);
            }
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _log.Error("Error Retrieving API Response: Not Found " + request.Resource);
                //throw new HttpException();
            }

            if (response.ErrorException != null)
            {
                _log.Error("Error Retrieving API Response: " + response.ErrorException);
                return default(TClass);
            }

            _log.Debug(request.Resource);
            return response.Data;
        }

        public bool ExecuteExpired<TClass>(RestRequest request) where TClass : new()
        {
            if (request == null)
            {
                return false;
            }
            var token = Task.Run(() => GetTokenAsync()).Result;
            request.AddHeader("Authorization", "bearer " + token);

            var response = _client.Execute<TClass>(request);

            if (response == null)
            {
                return false;
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                if (response.StatusDescription.Equals("Expired Token"))
                    return true;
            }

            return false;
        }
        public async Task<byte[]> ExecuteRawAsync(RestRequest request)
        {
            if (request == null)
            {
                _log.Error("Could Not Execute Raw API Request.  The Request was empty.");
                return null;
            }

            var token = await GetTokenAsync();
            request.AddHeader("Authorization", "bearer " + token);

            var response = await _client.DownloadDataAsync(request);

            if (response == null)
            {
                _log.Error("Could Not Complete Raw API Request.  The Response was empty." + request.Resource);
                return null;
            }

            _log.Debug(request.Resource);
            return response;
        }

        public string ExecuteRemotely(RestRequest request, string authHeader)
        {
            if (request == null)
            {
                _log.Error("Could Not Execute Remotely API Request.  The Request was empty.");
                return null;
            }

            request.AddHeader("Authorization", authHeader);

            var response = _client.Execute(request);
            if(response == null)
            {
                return null;
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                _log.Error("Could Not Complete Remotely API Request.  The Response Produced An Error." + request.Resource);
                _log.Error(response.Content);
                return null;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "The Remote Access Server Has Already Been Initialized";
            }

            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "Error: The Remotely Call Was Unauthorized";
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return "Error: " + response.Content;
            }

            _log.Error("Unknown Remotely Call Error");
            _log.Error(response.Content);
            return null;
        }

        public async Task<TClass> ExecuteHMACAsync<TClass>(RestRequest request, X509Certificate2 cert) where TClass : new()
        {
            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            string nonce = Guid.NewGuid().ToString("N");

            var url =
                System.Web.HttpUtility.UrlEncode(_client.Options.BaseUrl + request.Resource.ToLower());

            var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            string requestContentBase64String = string.Empty;
            if (body != null)
            {
                byte[] content = Encoding.ASCII.GetBytes(body.Value.ToString());
                MD5 md5 = MD5.Create();
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            string signatureRawData = String.Format("{0}{1}{2}{3}{4}", request.Method, url, requestTimeStamp, nonce, requestContentBase64String);
            var csp = (RSACryptoServiceProvider)cert.PrivateKey;
            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(signatureRawData);
            byte[] hash = sha1.ComputeHash(data);
            var signature = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));

            request.AddHeader("Authorization", "amx " + string.Format("{0}:{1}:{2}", Convert.ToBase64String(signature), nonce, requestTimeStamp));

            var response = await Task.Run(() => _client.ExecuteAsync<TClass>(request));
            if (response == null)
            {
                _log.Error("Could Not Complete API Request.  The Response was empty." + request.Resource);
                return default(TClass);
            }

            if (response.Data == null)
            {
                _log.Error("Response Data Was Null For Resource: " + request.Resource);
                return default(TClass);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                _log.Error("Could Not Complete API Request.  The Response Produced An Error." + request.Resource);
                _log.Error(response.Content);
                return default(TClass);
            }

            /*if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpException();
            }*/

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _log.Error("Error Retrieving API Response: Not Found " + request.Resource);
                //throw new HttpException();
            }

            if (response.ErrorException != null)
            {
                _log.Error("Error Retrieving API Response: " + response.ErrorException);
                return default(TClass);
            }

            _log.Debug(request.Resource);
            return response.Data;
        }
        
        public async Task<string> GetTokenAsync()
        {
            try
            {
                var authStateResult = await _protectedSessionStorage.GetItemAsync<AuthState>("authState");
                var claims = authStateResult.Claims.Select(c => new Claim(c.Type, c.Value));
                var token = claims.Where(x => x.Type.Equals("AccessToken", StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Value).FirstOrDefault();
                
                //var result = await _protectedSessionStorage.GetItemAsync<string>("authToken");
                //var token = result;
                return token;
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving token from ProtectedLocalStorage: {ex.Message}");
                return null;
            }
        }
    }
    
}