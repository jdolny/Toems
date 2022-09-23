using System;
using System.Net;
using System.Web;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class TokenApi
    {
        private readonly Uri _baseUrl;
        private readonly RestClient _client = new RestClient();
        private readonly ILog _log = LogManager.GetLogger(typeof(TokenApi));
        private readonly RestRequest _request = new RestRequest();
        private readonly string _resource;

        public TokenApi(string resource)
        {
            _resource = resource;
        }

        public TokenApi(Uri baseUrl, string resource)
        {
            _baseUrl = baseUrl;
            _resource = resource;
        }

        public DtoToken Get(string username, string password, string verificationCode=null)
        {
            var token = new DtoToken();

            _client.BaseUrl = _baseUrl ?? new Uri(HttpContext.Current.Request.Cookies["toemsBaseUrl"].Value);
            _request.Method = Method.POST;
            _request.Resource = string.Format("{0}", _resource);
            _request.AddParameter("grant_type", "password");
            _request.AddParameter("userName", username);
            _request.AddParameter("password", password);
            _request.AddParameter("verification_code", verificationCode);
            var response = _client.Execute<DtoToken>(_request);

            if (response == null)
            {
                _log.Error("Could Not Complete Token Request.  The Response was empty." + _request.Resource);
                token.error_description = "Did Not Receive A Response From The Auth Server.";
                return token;
            }

            if (response.Data != null)
            {
                token = response.Data;
            }

            if (response.ErrorException != null)
            {
                _log.Error("Error Obtaining Token: " + response.ErrorException);
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
                case 0:
                    token.error_description = "Could Not Contact API";
                    break;
                case HttpStatusCode.BadRequest:
                    token.error_description =
                        JsonConvert.DeserializeObject<DtoToken>(response.Content).error_description;
                    break;
                default:
                    token.error_description = "Unknown Error Obtaining Token";
                    _log.Error("Error Obtaining Token: " + response.Content);
                    break;
            }

            return token;
        }
    }
}