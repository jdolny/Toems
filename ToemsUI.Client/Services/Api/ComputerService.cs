using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toems_Common.Entity;

namespace ToemsUI.Client.Services.Api
{
    public class ComputerService
    {
        private ApiClient _apiClient;
        private string _resource;
        public ComputerService(string resource, string accessToken)
        {
            _resource = resource;
            _apiClient = new ApiClient() { AccessToken = accessToken };
        }

        public async Task<List<EntityComputer>> Get()
        {
            return await _apiClient.GetProtectedAsync<List<EntityComputer>>($"{_resource}/Get");
        }

        public async Task<EntityComputer> Get(string id)
        {
            return await _apiClient.GetProtectedAsync<EntityComputer>($"{_resource}/Get/{id}");
        }
    }
}
