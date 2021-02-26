using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Toems_Common.Dto;

namespace ToemsUI.Client.Services
{
    public class Authentication
    {
        private readonly string _baseUrl;

        public Authentication(string url)
        {
            _baseUrl = url;
        }

        public async Task<DtoToken> LoginUserAsync(DtoLoginRequest request)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/token");

            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("username", request.Username));
            keyValues.Add(new KeyValuePair<string, string>("password", request.Password));
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "password"));

            HttpContent content = new FormUrlEncodedContent(keyValues);
            message.Content = content;

            HttpClient client = new HttpClient();

            var response = await client.SendAsync(message);

            string jsonData = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<DtoToken>(jsonData);
        }
    }
}
