using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.pxeboot
{
    public partial class wiegen : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.PxeISOGen);
            if(!IsPostBack)
            {
                PopulateForm();
            }
        }

        private void PopulateForm()
        {
            txtTimezone.Text = "Eastern Standard Time";
            txtInput.Text = "0409:00000409";
            txtLanguage.Text = "en-us";
            gvServers.DataSource = Call.ClientComServerApi.Get();
            gvServers.DataBind();

            gvDrivers.DataSource = Call.FileCopyModuleApi.GetDriverList();
            gvDrivers.DataBind();


            PopulateProcess();
        }


        private void PopulateProcess()
        {
            var buildStatus = Call.WieBuildApi.GetRunningStatus();
            if (buildStatus != null)
            {
                lblRunning.Text = buildStatus;

            }
            
            var latestBuild = Call.WieBuildApi.GetLastBuild();
            if (latestBuild != null)
            {
                lblBuildDate.Text = latestBuild.EndTime.ToString();
                lblBuildOptions.Text = latestBuild.BuildOptions;
            }
        }
       
        protected void Timer_Tick(object sender, EventArgs e)
        {
            PopulateProcess();
            Call.WieBuildApi.UpdateStatus();
            UpdatePanel1.Update();
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            var config = new DtoWieConfig();
            config.Timezone = txtTimezone.Text;
            config.InputLocale = txtInput.Text;
            config.Language = txtLanguage.Text;
            config.Token = txtToken.Text;
            config.RestrictComServers = chkRestrictComServers.Checked;
            config.SkipAdkCheck = chkAdk.Checked;

            foreach (GridViewRow row in gvServers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvServers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var comServer = Call.ClientComServerApi.Get(Convert.ToInt32(dataKey.Value));
                if (comServer != null)
                    config.ComServers += "," + comServer.Url;
            }

           foreach (GridViewRow row in gvDrivers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvDrivers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                config.Drivers.Add(Convert.ToInt32(dataKey.Value));
            }

            config.ComServers = config.ComServers.Trim(',');
            var result = Call.SettingApi.GenerateWie(config);
            if (result.Success)
                EndUserMessage = "Successfully Started Build.  The Build Will Be Available In A Few Minutes.";
            else
                EndUserMessage = result.ErrorMessage;
        }

        protected void btnDownloadIso_Click(object sender, EventArgs e)
        {
            if(!Call.WieBuildApi.CheckIsoExists())
            {
                EndUserMessage = "WIE Iso could not be found.  Ensure the build has finished running.";
                return;
            }
            var iso = Call.WieBuildApi.ExportWie();
       
            Response.Clear();
            var ms = new MemoryStream(iso);
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("content-disposition", $"attachment;filename=WIE.iso");


            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
            EndUserMessage = "Download Complete.";
        }

        protected void btnDownloadTftp_Click(object sender, EventArgs e)
        {

        }
    }
}