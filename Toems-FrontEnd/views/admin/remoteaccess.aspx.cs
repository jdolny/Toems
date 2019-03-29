﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.admin
{
    public partial class remoteaccess : BasePages.Admin
    {
        public string DebugLevel { get; set; }
        public string Domain { get; set; }
        public string DomainUrl { get; set; }
        public string AuthCookie { get; set; }
        public string ServerDnsName { get; set; }
        public string ServerPublicPort { get; set; }
        public string PassRequirements { get; set; }
        public string ServerRedirectPort { get; set; }
        public string WebCertHash { get; set; }
        public string NodeId { get; set; }
        public string MeshServer { get; set; }


        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            var listSettings = new List<EntitySetting>
            {
                new EntitySetting
                {
                    Name = SettingStrings.RemoteAccessServer,
                    Value = txtRemoteAccessServer.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.RemoteAccessServer).Id
                }
            };

            EndUserMessage = Call.SettingApi.UpdateSettings(listSettings)
                ? "Successfully Updated Settings"
                : "Could Not Update Settings";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            NodeId = "node//";
            var mesh = GetSetting(SettingStrings.RemoteAccessServer);
            MeshServer = Utility.ReplaceHttp(mesh);
            if (IsPostBack)
            {
                if (Session["MeshLoginParams"] != null)
                {
                    var json = Session["MeshLoginParams"] as string;
                    var loginParams = JsonConvert.DeserializeObject<DtoRAServerLogin>(json);
                    DebugLevel = loginParams.debuglevel;
                    Domain = loginParams.domain;
                    DomainUrl = loginParams.domainurl;
                    AuthCookie = loginParams.authCookie;
                    ServerDnsName = loginParams.serverDnsName;
                    ServerPublicPort = loginParams.serverPublicPort;
                    PassRequirements = loginParams.passRequirements;
                    ServerRedirectPort = loginParams.serverRedirPort;
                    WebCertHash = loginParams.webcerthash;
                }
            }


            txtRemoteAccessServer.Text = mesh;


        }


        protected void btnCreateSession_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var cookies = new CookieContainer();
            ServicePointManager.Expect100Continue = false;

            var request = (HttpWebRequest)WebRequest.Create(GetSetting(SettingStrings.RemoteAccessServer) + "login");
            request.CookieContainer = cookies;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var requestStream = request.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write($"username=meshadmin&password={Call.SettingApi.GetMeshAdminPass()}&caller=toems");
            }


            var result = "";
            using (var responseStream = request.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                result = reader.ReadToEnd();
            }

            var loginParams = JsonConvert.DeserializeObject<DtoRAServerLogin>(result);
            if (loginParams == null) return;

            Session["MeshLoginParams"] = result;

            DebugLevel = loginParams.debuglevel;
            Domain = loginParams.domain;
            DomainUrl = loginParams.domainurl;
            AuthCookie = loginParams.authCookie;
            ServerDnsName = loginParams.serverDnsName;
            ServerPublicPort = loginParams.serverPublicPort;
            PassRequirements = loginParams.passRequirements;
            ServerRedirectPort = loginParams.serverRedirPort;
            WebCertHash = loginParams.webcerthash;
        }

        protected void SetupUsers_Click(object sender, EventArgs e)
        {
            var raServer = GetSetting(SettingStrings.RemoteAccessServer);
            if (string.IsNullOrEmpty(raServer))
            {
                EndUserMessage = "Remote Access Server Cannot Be Empty.  Ensure You Update Settings Before Creating The Mesh Session.";
                return;
            }

            try
            {

                var raStatus = (EnumRemoteAccess.RaStatus)Enum.Parse(typeof(EnumRemoteAccess.RaStatus), GetSetting(SettingStrings.RemoteAccessStatus));
                if (raStatus != EnumRemoteAccess.RaStatus.NotConfigured)
                {
                    EndUserMessage = "Users Have Already Been Created";
                    return;
                }
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var cookies = new CookieContainer();
                ServicePointManager.Expect100Continue = false;

                var adminPass = System.Web.Security.Membership.GeneratePassword(12, 0);
                var request = (HttpWebRequest)WebRequest.Create(raServer + "createaccount");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (var requestStream = request.GetRequestStream())
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write($"username=meshadmin&password1={adminPass}&password2={adminPass}&email=meshadmin@theopenem.local");
                }
                request.GetResponse();

                request = (HttpWebRequest)WebRequest.Create(raServer + "createaccount");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var controlPass = System.Web.Security.Membership.GeneratePassword(12, 0);
                using (var requestStream = request.GetRequestStream())
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write($"username=meshcontrol&password1={controlPass}&password2={controlPass}&email=meshcontrol@theopenem.local");
                }
                request.GetResponse();

                request = (HttpWebRequest)WebRequest.Create(raServer + "createaccount");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var viewPass = System.Web.Security.Membership.GeneratePassword(12, 0);
                using (var requestStream = request.GetRequestStream())
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write($"username=meshview&password1={viewPass}&password2={viewPass}&email=meshview@theopenem.local");
                }
                request.GetResponse();
                var listSettings = new List<EntitySetting>
                {
                    new EntitySetting
                    {
                        Name = SettingStrings.RemoteAccessAdminPasswordEncrypted,
                        Value = adminPass,
                        Id = Call.SettingApi.GetSetting(SettingStrings.RemoteAccessAdminPasswordEncrypted).Id
                    },
                     new EntitySetting
                    {
                        Name = SettingStrings.RemoteAccessControlPasswordEncrypted,
                        Value = controlPass,
                        Id = Call.SettingApi.GetSetting(SettingStrings.RemoteAccessControlPasswordEncrypted).Id
                    },
                      new EntitySetting
                    {
                        Name = SettingStrings.RemoteAccessViewPasswordEncrypted,
                        Value = viewPass,
                        Id = Call.SettingApi.GetSetting(SettingStrings.RemoteAccessViewPasswordEncrypted).Id
                    },
                       new EntitySetting
                    {
                        Name = SettingStrings.RemoteAccessStatus,
                        Value = "1",
                        Id = Call.SettingApi.GetSetting(SettingStrings.RemoteAccessStatus).Id
                    }

                };
                Call.SettingApi.UpdateSettings(listSettings);
                EndUserMessage = "Successfully Created Mesh Users";
            }
            catch (Exception ex)
            {
                EndUserMessage = "Could Not Create Mesh Users.  " + ex.Message;
            }
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            var meshId = meshID.Value.Replace("mesh//", "");

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.Expect100Continue = false;
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(new Uri($"https://{GetSetting(SettingStrings.RemoteAccessServer)}meshagents?id=3&meshid={meshId}&installflags=1"), "C:\\toems-local-storage\\software_uploads\\mesh.exe");
            }
        }
    }
}