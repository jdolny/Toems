using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToemsUI.Client.Services.Api
{
    public class ToemsApiService : IToemsApiService
    {
        public string AccessToken { get; set; }
        private readonly string _baseUrl;
      
        public ToemsApiService(string baseUrl)
        {           
            _baseUrl = baseUrl;
        }

        public ComputerService ComputerService
        {
            get { return new ComputerService($"{_baseUrl}/Computer",AccessToken); }
        }

        public CommandModuleService CommandModuleService
        {
            get { return new CommandModuleService($"{_baseUrl}/CommandModule", AccessToken); }
        }
    }
}
