using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toems_Common.Dto;

namespace ToemsUI.Services
{
    public class Authentication
    {

        private readonly string _baseUrl;

        ApiClient client = new ApiClient();

        public Authentication(string url)
        {
            _baseUrl = url;
        }

        public async Task<DtoToken> LoginUserAsync(DtoLoginRequest request)
        {
            var response = await client.LoginUserAsync($"{_baseUrl}/token", request);
            return response;
        }

        /*public async Task<DtoToken> LoginUserAsync(LoginRequest request)
        {
            var _client = new RestClient();
            var _request = new RestRequest();
            var token = new DtoToken();

            _client.BaseUrl = new Uri(_baseUrl);
            _request.Method = Method.POST;
            _request.Resource = "/token";
            _request.AddParameter("grant_type", "password");
            _request.AddParameter("userName", request.Username);
            _request.AddParameter("password", request.Password);

            var response = await _client.ExecuteAsync<DtoToken>(_request);

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
        }*/

    }
}
