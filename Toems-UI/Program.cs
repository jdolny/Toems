using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using Toems_ApiCalls;
using Toems_ServiceCore.Data;
using Toems_UI.Services.ControllerService;
using Toems_UI.Site;

log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddMudBlazorSnackbar(config =>
{
    config.PositionClass = Defaults.Classes.Position.TopCenter; 
    config.PreventDuplicates = true;
    config.NewestOnTop = true;
    config.ShowCloseIcon = true;
    config.VisibleStateDuration = 10000;
    config.HideTransitionDuration = 500;
    config.ShowTransitionDuration = 500;
    config.ClearAfterNavigation = true;
    config.SnackbarVariant = Variant.Filled;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<ToemsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ToemsConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToemsConnection"))));

builder.Services.AddScoped<IToemsDbFactory, ToemsDbFactory>();

builder.Services.AddHttpClient("API", client => client.BaseAddress = new Uri(builder.Configuration["ApplicationApiUrl"]));
builder.Services.AddServerSideBlazor(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);
});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));
builder.Services.AddScoped<LocalAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<LocalAuthStateProvider>());
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<ApiRequest>();
builder.Services.AddScoped<APICall>();
builder.Services.AddScoped<DebugService>(); 
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();





builder.Services.AddOpenApi();



var app = builder.Build();
app.MapControllers();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();
