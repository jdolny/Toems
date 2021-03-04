using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toems_Common.Entity;

namespace ToemsUI.Client.Services.Api
{
    public class CategoryService
    {
        private ApiClient _apiClient;
        private string _resource;
        public CategoryService(string resource, string accessToken)
        {
            _resource = resource;
            _apiClient = new ApiClient() { AccessToken = accessToken };
        }

        public async Task<List<EntityCategory>> Get()
        {
            return await _apiClient.GetProtectedAsync<List<EntityCategory>>($"{_resource}/Get");
        }

        public async Task<EntityCategory> Get(string id)
        {
            return await _apiClient.GetProtectedAsync<EntityCategory>($"{_resource}/Get/{id}");
        }
    }
}
