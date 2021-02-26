using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toems_Common.Entity;
using Toems_Common.Dto;

namespace ToemsUI.Client.Services.Api
{
    public class CommandModuleService
    {
        private ApiClient _apiClient;
        private string _resource;
        public CommandModuleService(string resource, string accessToken)
        {
            _resource = resource;
            _apiClient = new ApiClient() { AccessToken = accessToken };
        }

        public async Task<List<EntityCommandModule>> Get()
        {
            return await _apiClient.GetProtectedAsync<List<EntityCommandModule>>($"{_resource}/Get");
        }

        public async Task<EntityCommandModule> Get(string id)
        {
            return await _apiClient.GetProtectedAsync<EntityCommandModule>($"{_resource}/Get/{id}");
        }

        public async Task<DtoActionResult> Delete(int id)
        {
            return await _apiClient.DeleteProtectedAsync($"{_resource}/Delete/{id}");
        }
    }
}
