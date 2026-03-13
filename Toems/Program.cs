using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using Toems_ApiCalls;
using Toems_ClientApi.Controllers.Authorization;
using Toems_ClientApi.Hubs;
using Toems_ClientApi.Hubs.Authorization;
using Toems_DataModel;
using Toems_ServiceCore.Data;
using Toems_ServiceCore.Infrastructure;
using Toems_UI;
using Toems_UI.Services;
using Toems_UI.Services.ControllerService;
using Toems.Site;
using ToemsDbContext = Toems_ServiceCore.Data.ToemsDbContext;

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

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
builder.Services.AddSingleton<ILog>(LogManager.GetLogger(typeof(Program)));

builder.Services.Scan(scan => scan
    .FromAssemblyOf<AuthenticationService>()  // pick any type from your services assembly
    .AddClasses(classes => classes.InNamespaces("Toems_ServiceCore.EntityServices"))
    .AddClasses(classes => classes.InNamespaces("Toems_ServiceCore.Infrastructure"))
    .AddClasses(classes => classes.InNamespaces("Toems_ServiceCore.Workflows"))
    .AsSelf()
    .WithScopedLifetime());

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<ToemsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ToemsConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToemsConnection"))));

builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<ToemsDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();    

builder.Services.AddScoped<IToemsDbFactory, ToemsDbFactory>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<ComputerService>();

builder.Services.AddHttpClient("API", client => client.BaseAddress = new Uri(builder.Configuration["ApplicationApiUrl"]));
builder.Services.AddServerSideBlazor(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);
});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));
//builder.Services.AddScoped<LocalAuthStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<LocalAuthStateProvider>());
//builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<ApiRequest>();
builder.Services.AddScoped<APICall>();
builder.Services.AddScoped<DebugService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddScoped<ActionHubService>();
builder.Services.AddScoped<HubCertAuth>();
builder.Services.AddScoped<CertificateAuthFilter>();
builder.Services.AddScoped<ClientImagingAuthFilter>();
builder.Services.AddScoped<InterComAuthFilter>();
builder.Services.AddScoped<ProvisionAuthFilter>();
builder.Services.AddMemoryCache();

builder.Services.AddSignalR(options =>
{
    //Applies HubCertAuth to all hubs. Can be overridden by adding [HubAuthorize(false)] to a hub, which will skip the filter for that hub
    //options.AddFilter<HubCertAuth>();
});





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
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorPages();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToemsDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
