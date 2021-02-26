using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;

namespace ToemsUI.Client.Services.Api
{
    public class ApiClient
    {
        public string AccessToken { get; set; }

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
            if (string.IsNullOrEmpty(AccessToken))
                return default;

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
            if (string.IsNullOrEmpty(AccessToken))
                return default;

            HttpClient client = new HttpClient();
     
            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            // Send a request and get the response 

                var response = await client.GetAsync(methodUrl);
                var jsonData = await response.Content.ReadAsStringAsync();

                T obj = JsonConvert.DeserializeObject<T>(jsonData);

                return obj;

            // Read the data 
          
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
            if (string.IsNullOrEmpty(AccessToken))
                return default;
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
            if (string.IsNullOrEmpty(AccessToken))
                return default;

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
        public async Task<DtoActionResult> DeleteProtectedAsync(string methodUrl)
        {
            if (string.IsNullOrEmpty(AccessToken))
                return default;

            HttpClient client = new HttpClient();

            // Set the access token for the request 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await client.DeleteAsync(methodUrl);
            var jsonData = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<DtoActionResult>(jsonData);
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
            if (string.IsNullOrEmpty(AccessToken))
                return default;

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
}
