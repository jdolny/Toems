using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Hangfire;
using Hangfire.MySql;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Toems_ApplicationApi;
using Toems_Common;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>; 

[assembly: OwinStartup(typeof (Startup))]

namespace Toems_ApplicationApi
{
    public class CustomAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket != null &&
                context.Ticket.Properties.ExpiresUtc.HasValue &&
                context.Ticket.Properties.ExpiresUtc.Value.LocalDateTime < DateTime.Now)
            {

                context.OwinContext.Set("custom.ExpiredToken", true);
            }
        }
    }

    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var data = context.Request.ReadFormAsync();
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] {"*"});
            var auth = new AuthenticationServices();

            var validationResult = auth.GlobalLogin(context.UserName, context.Password, "Web", data.Result["verification_code"]);
            if (validationResult.Success)
            {
                var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                context.Validated(oAuthIdentity);
                var user = new ServiceUser().GetUser(context.UserName);
                oAuthIdentity.AddClaim(new Claim("user_id", user.Id.ToString()));
                if(validationResult.ErrorMessage.Equals("Mfa setup is required"))
                    oAuthIdentity.AddClaim(new Claim("mfa_setup_required", "true"));
                //set different time spans here
                //if (user.Membership == "Administrator")
                //    context.Options.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(20);
                context.Validated(oAuthIdentity);
            }
            else
            {
                context.SetError("invalid_grant", validationResult.ErrorMessage);
            }
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }
    }


    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            var OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new SimpleAuthorizationServerProvider()
            };

            var webTimeout = ServiceSetting.GetSettingValue(SettingStrings.WebUiTimeout);
            if(!string.IsNullOrEmpty(webTimeout))
            {
                int timeoutInt = 0;
                bool result = int.TryParse(webTimeout, out timeoutInt);
                if (result)
                    OAuthServerOptions.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(timeoutInt);
            }
            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            {
                AccessTokenProvider = new CustomAuthenticationTokenProvider()
            });

            app.Use(new Func<AppFunc, AppFunc>(next => (env) =>
            {
                var ctx = new OwinContext(env);
                if (ctx.Get<bool>("custom.ExpiredToken"))
                {
                    ctx.Response.StatusCode = 403;
                    ctx.Response.ReasonPhrase = "Expired Token";

                    return Task.FromResult(0);
                }
                else
                {
                    return next(env);
                }
            }));

            // Hangfire initialization
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            GlobalJobFilters.Filters.Add(new DisableConcurrentExecutionAttribute(600));

            var providerName = System.Configuration.ConfigurationManager.
                ConnectionStrings["toems"].ProviderName;

            if (providerName.Equals("MySql.Data.MySqlClient"))
            {              
                //mysql
                Hangfire.GlobalConfiguration.Configuration.UseStorage(
                    new MySqlStorage(ConfigurationManager.
                        ConnectionStrings["toems"].ConnectionString, new MySqlStorageOptions()));
            }
            else
            {
                //sql server
                Hangfire.GlobalConfiguration.Configuration.UseStorage(
                new SqlServerStorage(ConfigurationManager.
                    ConnectionStrings["toems"].ConnectionString));
            }

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        
            try
            {
                if (ConfigurationManager.AppSettings["ServerRole"].ToLower().Equals("primary"))
                {
                    RecurringJob.AddOrUpdate("ScheduleRunner-Job", () => new ScheduleRunner().Run(), 
                       "*/15 * * * *", TimeZoneInfo.Local);

                    var imagingTimeout = ServiceSetting.GetSettingValue(SettingStrings.ImageTaskTimeoutMinutes);
                    if(!string.IsNullOrEmpty(imagingTimeout))
                    {
                        if(!imagingTimeout.Equals("0"))
                            RecurringJob.AddOrUpdate("ImagingTaskTimeout-Job", () => new ServiceActiveImagingTask().CancelTimedOutTasks(),
                      "*/5 * * * *", TimeZoneInfo.Local);
                    }

                    var path = HttpContext.Current.Server.MapPath("~");
                    RecurringJob.AddOrUpdate("Toec-Deploy-Job", () => new ToecRemoteInstaller().Run(path), "*/15 * * * *", TimeZoneInfo.Local);

                    var groupSchedule = ServiceSetting.GetSettingValue(SettingStrings.DynamicGroupSchedule);
                    var ldapSchedule = ServiceSetting.GetSettingValue(SettingStrings.LdapSyncSchedule);
                    var storageSchedule = ServiceSetting.GetSettingValue(SettingStrings.FolderSyncSchedule);
                    var resetSchedule = ServiceSetting.GetSettingValue(SettingStrings.ResetReportSchedule);
                    var approvalSchedule = ServiceSetting.GetSettingValue(SettingStrings.ApproveReportSchedule);
                    var smartSchedule = ServiceSetting.GetSettingValue(SettingStrings.SmartReportSchedule);
                    var dataCleanupSchedule = ServiceSetting.GetSettingValue(SettingStrings.DataCleanupSchedule);
                    var lowDiskSchedule = ServiceSetting.GetSettingValue(SettingStrings.LowDiskSchedule);

                    RecurringJob.AddOrUpdate("DynamicGroupUpdate-Job", () => new UpdateDynamicMemberships().All(),
                        groupSchedule, TimeZoneInfo.Local);
                    RecurringJob.AddOrUpdate("StorageSync-Job", () => new FolderSync().RunAllServers(), storageSchedule,
                        TimeZoneInfo.Local);
                    RecurringJob.AddOrUpdate("LDAPSync-Job", () => new LdapSync().Run(), ldapSchedule,
                        TimeZoneInfo.Local);

                    if (!string.IsNullOrEmpty(resetSchedule))
                        RecurringJob.AddOrUpdate("ResetRequestReport-Job",
                            () => new ServiceResetRequest().SendResetRequestReport(), resetSchedule, TimeZoneInfo.Local);
                    else
                        RecurringJob.RemoveIfExists("ResetRequestReport-Job");

                    if (!string.IsNullOrEmpty(approvalSchedule))
                        RecurringJob.AddOrUpdate("ApprovalRequestReport-Job",
                            () => new ServiceApprovalRequest().SendApprovalRequestReport(), approvalSchedule,
                            TimeZoneInfo.Local);
                    else
                        RecurringJob.RemoveIfExists("ApprovalRequestReport-Job");

                    if (!string.IsNullOrEmpty(smartSchedule))
                        RecurringJob.AddOrUpdate("SmartReport-Job", () => new ServiceReport().SendSmartReport(),
                            smartSchedule, TimeZoneInfo.Local);
                    else
                        RecurringJob.RemoveIfExists("SmartReport-Job");

                    if (!string.IsNullOrEmpty(lowDiskSchedule))
                        RecurringJob.AddOrUpdate("LowDiskReport-Job", () => new ServiceReport().SendLowDiskSpaceReport(),
                      lowDiskSchedule, TimeZoneInfo.Local);
                    else
                        RecurringJob.RemoveIfExists("LowDiskReport-Job");


                    if (!string.IsNullOrEmpty(dataCleanupSchedule))
                        RecurringJob.AddOrUpdate("DataCleanup-Job", () => new DataCleanup().Run(),
                            dataCleanupSchedule, TimeZoneInfo.Local);
                    else
                        RecurringJob.RemoveIfExists("DataCleanup-Job");
                }

            }
            catch
            {
                // ignored
            }
        }
    }

   
}