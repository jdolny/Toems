using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToemsUI.Client.Services.Api;

namespace ToemsUI.Client.Services
{
    public class UserState
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ToemsApiService _toemsApiService;
        public UserState(AuthenticationStateProvider authenticationStateProvider, ToemsApiService toemsApiService)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _toemsApiService = toemsApiService;
        }

        public async Task<bool> SetAccessToken()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (authState == null) return false;
            var claim = authState.User.FindFirst("AccessToken");
            if (claim == null) return false;
            _toemsApiService.AccessToken = claim.Value;
            return true;

        }
    }
}
