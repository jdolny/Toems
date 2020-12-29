﻿using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.computers
{
    public partial class remoteaccess : BasePages.Computers
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

        protected void Page_Load(object sender, EventArgs e)
        {
            /*ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var cookies = new CookieContainer();
            ServicePointManager.Expect100Continue = false;
            var pass = Call.SettingApi.GetMeshAdminPass();
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
            NodeId = "node//" + ComputerEntity.RemoteAccessId;
            MeshServer = GetSetting(SettingStrings.RemoteAccessServer);*/
          
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ComputerEntity.RemoteAccessId))
            {
                EndUserMessage = "Cannot Start Remote Control.  The Remote Control Agent Is Not Installed On This Computer.";
                return;
            }
            var url = Call.RemoteAccessApi.GetRemoteControlUrl(ComputerEntity.RemoteAccessId);
            if (string.IsNullOrEmpty(url))
                EndUserMessage = "Unknown Error.  Check The Exception Logs.";
            else if (url.Contains("Error"))
                EndUserMessage = url;
            else if (url.Contains("http") || url.Contains("https"))
            {
                string redirect = $"<script>window.open('{url}');</script>";
                frame.Attributes.Add("src", url);
            }

        }
    }
}