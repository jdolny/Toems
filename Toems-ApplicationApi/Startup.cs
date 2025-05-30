﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Hangfire;
using Hangfire.MySql;
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
                {
                    if (timeoutInt == 0) timeoutInt = 99999;
                    OAuthServerOptions.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(timeoutInt);
                }
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

       
                //mysql
                Hangfire.GlobalConfiguration.Configuration.UseStorage(
                    new MySqlStorage(ConfigurationManager.
                        ConnectionStrings["toems"].ConnectionString, new MySqlStorageOptions()));


            app.UseHangfireDashboard();
            app.UseHangfireServer();
        
            try
            {
                if (ConfigurationManager.AppSettings["ServerRole"].ToLower().Equals("primary"))
                {
                   

                    RecurringJob.AddOrUpdate("ScheduleRunner-Job", () => new ScheduleRunner().Run(), "*/15 * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

                    var imagingTimeout = ServiceSetting.GetSettingValue(SettingStrings.ImageTaskTimeoutMinutes);
                    if(!string.IsNullOrEmpty(imagingTimeout))
                    {
                        if(!imagingTimeout.Equals("0"))
                            RecurringJob.AddOrUpdate("ImagingTaskTimeout-Job", () => new ServiceActiveImagingTask().CancelTimedOutTasks(),
                      "*/5 * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    }

                    var path = HttpContext.Current.Server.MapPath("~");
                    RecurringJob.AddOrUpdate("Toec-Deploy-Job", () => new ToecRemoteInstaller().Run(path), "*/15 * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

                    var groupSchedule = ServiceSetting.GetSettingValue(SettingStrings.DynamicGroupSchedule);
                    var ldapSchedule = ServiceSetting.GetSettingValue(SettingStrings.LdapSyncSchedule);
                    var storageSchedule = ServiceSetting.GetSettingValue(SettingStrings.FolderSyncSchedule);
                    var resetSchedule = ServiceSetting.GetSettingValue(SettingStrings.ResetReportSchedule);
                    var approvalSchedule = ServiceSetting.GetSettingValue(SettingStrings.ApproveReportSchedule);
                    var smartSchedule = ServiceSetting.GetSettingValue(SettingStrings.SmartReportSchedule);
                    var dataCleanupSchedule = ServiceSetting.GetSettingValue(SettingStrings.DataCleanupSchedule);
                    var lowDiskSchedule = ServiceSetting.GetSettingValue(SettingStrings.LowDiskSchedule);
                    var wingetSchedule = ServiceSetting.GetSettingValue(SettingStrings.WingetManifestSchedule);

                    RecurringJob.AddOrUpdate("DynamicGroupUpdate-Job", () => new UpdateDynamicMemberships().All(),
                        groupSchedule, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    RecurringJob.AddOrUpdate("StorageSync-Job", () => new FolderSync().RunAllServers(), storageSchedule,
                        new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    RecurringJob.AddOrUpdate("LDAPSync-Job", () => new LdapSync().Run(), ldapSchedule,
                        new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

                    if (!string.IsNullOrEmpty(resetSchedule))
                        RecurringJob.AddOrUpdate("ResetRequestReport-Job",
                            () => new ServiceResetRequest().SendResetRequestReport(), resetSchedule, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    else
                        RecurringJob.RemoveIfExists("ResetRequestReport-Job");

                    if (!string.IsNullOrEmpty(approvalSchedule))
                        RecurringJob.AddOrUpdate("ApprovalRequestReport-Job",
                            () => new ServiceApprovalRequest().SendApprovalRequestReport(), approvalSchedule,
                            new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    else
                        RecurringJob.RemoveIfExists("ApprovalRequestReport-Job");

                    if (!string.IsNullOrEmpty(smartSchedule))
                        RecurringJob.AddOrUpdate("SmartReport-Job", () => new ServiceReport().SendSmartReport(),
                            smartSchedule, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    else
                        RecurringJob.RemoveIfExists("SmartReport-Job");

                    if (!string.IsNullOrEmpty(lowDiskSchedule))
                        RecurringJob.AddOrUpdate("LowDiskReport-Job", () => new ServiceReport().SendLowDiskSpaceReport(),
                      lowDiskSchedule, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    else
                        RecurringJob.RemoveIfExists("LowDiskReport-Job");


                    if (!string.IsNullOrEmpty(dataCleanupSchedule))
                        RecurringJob.AddOrUpdate("DataCleanup-Job", () => new DataCleanup().Run(),
                            dataCleanupSchedule, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    else
                        RecurringJob.RemoveIfExists("DataCleanup-Job");

                    if(!string.IsNullOrEmpty(wingetSchedule))
                        RecurringJob.AddOrUpdate("WingetManifest-Job", () => new WinGetManifestImporter().Run(path),
                            wingetSchedule, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                    else
                        RecurringJob.RemoveIfExists("WingetManifest-Job");
                }

            }
            catch
            {
                // ignored
            }
        }
    }

   
}