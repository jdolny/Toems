using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Toems_Common.Dto;

namespace ToemsUI
{
    public class LocalAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _storageService;

        public LocalAuthenticationStateProvider(ILocalStorageService storageService)
        {
            _storageService = storageService;
        }


        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if(await _storageService.ContainKeyAsync("User"))
            {
                var userInfo = await _storageService.GetItemAsync<DtoToken>("User");

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.username),
                    new Claim("Membership", userInfo.membership),
                    new Claim("Theme", userInfo.theme),
                    new Claim("AccessToken", userInfo.access_token),

                };

                var identity = new ClaimsIdentity(claims, "BearerToken");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }

            return new AuthenticationState(new ClaimsPrincipal()); //return empty claims for no user found in local storage
        }
    }
}
