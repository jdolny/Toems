//https://github.com/aksoftware98/aksoftware.webapi/blob/master/WebApi.Client/ServiceClient.cs

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Toems_Common.Dto;

namespace ToemsUI.Services
{
    public class ApiClient
    {
        public string AccessToken { get; set; }

        // Method to login and get access token

        /// <summary>
        /// Login a user by verifying the username and password and return the access token for this login
        /// </summary>
        /// <param name="url">URL of your Web API that represetns the login function </param>
        /// <param name="userName">Username </param>
        /// <param name="password">Password </param>
        /// <returns></returns>
        public async Task<DtoToken> LoginUserAsync(string url, DtoLoginRequest request)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);

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


        // Methods to invoke normal WebApi methods
        #region NormalMethods
        // Method to invoke a post method 

        /// <summary>
        /// Make an unauthorized POST request to your web api and return a specific model 
        /// </summary>
        /// <typeparam name="T">Your data type that you want to send and get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the POST function </param>
        /// <param name="methodUrl">The Method name in the URL of your data source in the web api</param>
        /// <param name="model"> The model or object that you want to send</param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string methodUrl, object model)
        {
            HttpClient client = new HttpClient();

            // Now serialzize the object to json 
            string jsonData = JsonConvert.SerializeObject(model);

            // Create a content 
            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            // Make a request 
            var response = await client.PostAsync(methodUrl, content);
            var responseAsString = await response.Content.ReadAsStringAsync();

            // Deserialize the coming object into a T object 
            T obj = JsonConvert.DeserializeObject<T>(responseAsString);

            return obj;
        }

        // Method to invoke a get method 

        /// <summary>
        /// Make an unauthorized GET request to your web api and return a specific model
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the GET function </param>
        /// <param name="methodUrl">The Method name in the URL of your data source in the web api</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string methodUrl)
        {
            HttpClient client = new HttpClient();

            // Send a request and get the response 
            var response = await client.GetAsync(methodUrl);
            // Read the data 
            var jsonData = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(jsonData);

            return obj;
        }

        /// <summary>
        /// Make an unauthorized GET request to your web api and return the result as a raw json
        /// </summary>
        /// <param name="methodUrl">URL of your Web API that represetns the GET function </param>
        /// <returns></returns>
        public async Task<string> GetJsonResultAsync(string methodUrl)
        {
            HttpClient client = new HttpClient();

            // Send requant the get the response 
            var response = await client.GetAsync(methodUrl);
            // Read the data 
            var jsonData = await response.Content.ReadAsStringAsync();
            return jsonData;
        }

        // Method to invoke a put method 

        /// <summary>
        /// Make an unauthorized PUT request to your web api to alter a specific data model
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the PUT function </param>
        /// <param name="model">The model or object that you want to alter</param>
        /// <param name="id">The id of the object that you want to alter</param>
        /// <returns></returns>
        public async Task<bool> PutAsync<T>(string methodUrl, object model, int id)
        {
            HttpClient client = new HttpClient();

            // Serialize the object
            string jsonData = JsonConvert.SerializeObject(model);

            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync(methodUrl + "/" + id, content);

            return response.IsSuccessStatusCode;
        }


        /// <summary>
        /// Make an unauthorized PUT request to your web api to alter a specific data model
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the PUT function </param>
        /// <param name="model">The model or object that you want to alter</param>
        /// <param name="id">The id of the object that you want to alter</param>
        /// <returns></returns>
        public async Task<T> PutAsync<T>(string methodUrl, object model)
        {
            HttpClient client = new HttpClient();

            // Serialize the object
            string jsonData = JsonConvert.SerializeObject(model);

            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync(methodUrl, content);

            // Read the data 
            var responseString = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(responseString);

            return obj;
        }

        // Method to invoke the delete moethod 

        /// <summary>
        /// Make an unauthorized DELETE request to your web api to delete a specific object
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the DELETE function </param>
        /// <param name="id">The id of the object that you want to delete</param>
        /// <returns></returns>
        public async Task<T> DeleteAsync<T>(string methodUrl, int id)
        {
            HttpClient client = new HttpClient();

            var response = await client.DeleteAsync(methodUrl + "/" + id);
            var jsonData = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(jsonData);

            return obj;
        }


        /// <summary>
        /// Make an unauthorized DELETE request to your web api to delete a specific object
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the DELETE function </param>
        /// <param name="model">Instance of the object that you want to send it to the API to Delete </param>
        /// <returns></returns>
        public async Task<T> DeleteAsync<T>(string methodUrl, object model)
        {
            HttpClient client = new HttpClient();

            string jsonData = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.DeleteAsync(methodUrl);
            var responseString = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(jsonData);

            return obj;
        }
        #endregion

        // Methods to invoke protected WebApi methods 
        #region ProtectedMethods
        // Method to invoke a post method 

        /// <summary>
        /// Make a protected POST request to your web api and return a specific model 
        /// </summary>
        /// <typeparam name="T">Your data type that you want to send and get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the protected POST function </param>
        /// <param name="model"> The model or object that you want to send</param>
        /// <returns></returns>
        public async Task<T> PostProtectedAsync<T>(string methodUrl, object model)
        {
            HttpClient client = new HttpClient();

            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            // Now serialzize the object to json 
            string jsonData = JsonConvert.SerializeObject(model);

            // Create a content 
            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            // Make a request 
            var response = await client.PostAsync(methodUrl, content);
            var responseAsString = await response.Content.ReadAsStringAsync();

            // Deserialize the coming object into a T object 
            T obj = JsonConvert.DeserializeObject<T>(responseAsString);

            return obj;
        }

        // Method to invoke a get method protected

        /// <summary>
        /// Make a protected GET request to your web api and return a specific model
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the GET function </param>
        /// <returns></returns>
        public async Task<T> GetProtectedAsync<T>(string methodUrl)
        {
            HttpClient client = new HttpClient();

            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            // Send a request and get the response 
            var response = await client.GetAsync(methodUrl);
            // Read the data 
            var jsonData = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(jsonData);

            return obj;
        }

        // Method to invoke the Get and return the json result without desiralizing 
        /// <summary>
        /// Make an authorized GET Request to your data source 
        /// </summary>
        /// <param name="methodUrl">URL of your Web API that represetns the gET function </param>
        /// <returns></returns>
        public async Task<string> GetProtectedJsonResultAsync(string methodUrl)
        {
            HttpClient client = new HttpClient();

            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            // Send a request and get the response 
            var response = await client.GetAsync(methodUrl);
            // Read the data 
            var jsonData = await response.Content.ReadAsStringAsync();

            return jsonData;
        }


        // Method to invoke a put method protected

        /// <summary>
        /// Make a protected PUT request to your web api to alter a specific data model
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the PUT function </param>
        /// <param name="model">The model or object that you want to alter</param>
        /// <param name="id">The id of the object that you want to alter</param>
        /// <returns></returns>
        public async Task<T> PutProtectedAsync<T>(string methodUrl, object model, int id)
        {
            HttpClient client = new HttpClient();

            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            // Serialize the object
            string jsonData = JsonConvert.SerializeObject(model);

            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync(methodUrl + "/" + id, content);

            var responseString = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(responseString);

            return obj;
        }

        /// <summary>
        /// Make a protected PUT request to your web api to alter a specific data model
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the PUT function </param>
        /// <param name="model">The model or object that you want to alter</param>
        /// <returns></returns>
        public async Task<T> PutProtectedAsync<T>(string methodUrl, object model)
        {
            HttpClient client = new HttpClient();

            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            // Serialize the object
            string jsonData = JsonConvert.SerializeObject(model);

            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync(methodUrl, content);

            var responseString = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(responseString);

            return obj;
        }

        // Method to invoke the delete moethod protected 

        /// <summary>
        /// Make a protected DELETE request to your web api to delete a specific object
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the DELETE function </param>
        /// <param name="id">The id of the object that you want to delete</param>
        /// <returns></returns>
        public async Task<T> DeleteProtectedAsync<T>(string methodUrl, int id)
        {
            HttpClient client = new HttpClient();

            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await client.DeleteAsync(methodUrl + "/" + id);
            var jsonData = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(jsonData);

            return obj;
        }


        /// <summary>
        /// Make a protected DELETE request to your web api to delete a specific object
        /// </summary>
        /// <typeparam name="T">Your data type that you want to get</typeparam>
        /// <param name="methodUrl">URL of your Web API that represetns the DELETE function </param>
        /// <param name="model">The model or object that you want to delete</param>
        /// <returns></returns>
        public async Task<T> DeleteProtectedAsync<T>(string methodUrl, object model)
        {
            HttpClient client = new HttpClient();

            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            string jsonData = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.DeleteAsync(methodUrl);
            var responseString = await response.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(jsonData);

            return obj;
        }
        #endregion

    }


    internal class UserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

