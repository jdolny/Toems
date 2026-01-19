using System.Text;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Toems_ApplicationApiCore;
using Toems_Service;
using Microsoft.Extensions.DependencyInjection;

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

builder.Services.Scan(scan => scan
    .FromAssemblyOf<AuthenticationServices>()  // pick any type from your services assembly
    .AddClasses(classes => classes.InNamespaces("Toems_Service.Entity"))
    .AsSelf()
    .WithScopedLifetime());
builder.Services.AddControllers();

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

