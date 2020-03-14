using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.logs
{
    public partial class comserver : BasePages.Admin
    {
        protected void btnExportLog_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AppendHeader("Content-Disposition",
                "attachment; filename=" + ddlLog.Text);
            var log = Call.FilesystemApi.GetComServerLogContents(ddlLog.Text, int.MaxValue, Convert.ToInt32(ddlComServer.SelectedValue));
            if (log == null) return;
            var sb = new StringBuilder();
            foreach (var line in log)
            {
                sb.Append(line);
                sb.Append(Environment.NewLine);
            }
            HttpContext.Current.Response.Write(sb.ToString());
            HttpContext.Current.Response.End();
        }

        protected void ddlLimit_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateLogs();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PopulateComServers(ddlComServer);
                    var comLogs = Call.FilesystemApi.GetComServerLogs(Convert.ToInt32(ddlComServer.SelectedValue));
                    if (comLogs != null)
                    {
                        ddlLog.DataSource = comLogs;
                        ddlLog.DataBind();
                        ddlLog.Items.Insert(0, "Select A Log");
                    }
                    ddlLimit.SelectedValue = "25";

                }
                PopulateLogs();
            }
            catch { }
        }

        private void PopulateLogs()
        {
            if (ddlLog.Text != "Select A Log")
            {
                var limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
                gvLog.DataSource = Call.FilesystemApi.GetComServerLogContents(ddlLog.Text, limit, Convert.ToInt32(ddlComServer.SelectedValue));
                gvLog.DataBind();
            }
        }
    }
}