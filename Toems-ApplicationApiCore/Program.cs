using System.Reflection;
using System.Text;
using Hangfire;
using Hangfire.MySql;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Toems_ApplicationApiCore;
using Microsoft.Extensions.DependencyInjection;
using Toems_DataModel;
using Toems_ServiceCore.Data;
using Toems_ServiceCore.Infrastructure;
using ToemsDbContext = Toems_ServiceCore.Data.ToemsDbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(configuration =>
{
    configuration.UseStorage(
        new MySqlStorage(
            builder.Configuration.GetConnectionString("HangfireConnection"), new MySqlStorageOptions()));

});
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1; // Set to 1 to process one job at a time
});

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

builder.Services.AddScoped<UnitOfWork>();   
builder.Services.AddScoped<IToemsDbFactory, ToemsDbFactory>();
builder.Services.AddHttpContextAccessor();

// Configure JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            //ValidateIssuerSigningKey = true,
            //IssuerSigningKey = new SymmetricSecurityKey(
             //   Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        // Custom expired token handling (like your old CustomAuthenticationTokenProvider)
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Expired Token\"}");
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

