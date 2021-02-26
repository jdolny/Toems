using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Radzen;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ToemsUI.Client.Services.Api;
using ToemsUI.Client.Services;

namespace ToemsUI.Client
{
    public class ThemeState
    {
        public string CurrentTheme { get; set; } = "default";
    }
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var url = builder.Configuration.GetValue<string>("Toems-API");


            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<AuthenticationStateProvider, LocalAuthenticationStateProvider>();
            builder.Services.AddScoped<UserState>();
            builder.Services.AddScoped<ThemeState>();
            builder.Services.AddScoped<DefaultNavigationService>();
            builder.Services.AddScoped<Authentication>(services =>
            {
                return new Authentication(url);
            });

            builder.Services.AddScoped<ToemsApiService>(s =>
            {
                return new ToemsApiService(url);
            });

            
            builder.RootComponents.Add<App>("#app");
            await builder.Build().RunAsync();
        }
    }
}
