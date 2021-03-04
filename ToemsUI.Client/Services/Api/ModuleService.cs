using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toems_Common.Entity;
using Toems_Common.Dto;
using Toems_Common.Enum;

namespace ToemsUI.Client.Services.Api
{
    public class ModuleService
    {
        private ApiClient _apiClient;
        private string _resource;
        public ModuleService(string resource, string accessToken)
        {
            _resource = resource;
            _apiClient = new ApiClient() { AccessToken = accessToken };
        }

        public async Task<DtoActionResult> Archive(int moduleId, EnumModule.ModuleType moduleType)
        {
            return await _apiClient.GetProtectedAsync<DtoActionResult>($"{_resource}/Archive/?moduleId={moduleId}&moduleType={moduleType}");
        }
    }
}
