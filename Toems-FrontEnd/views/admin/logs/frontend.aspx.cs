using System;
using System.Text;
using System.Web;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin.logs
{
    public partial class frontend : Admin
    {
        protected void btnExportLog_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AppendHeader("Content-Disposition",
                "attachment; filename=" + ddlLog.Text);
            var log = GetLogContents(ddlLog.Text, int.MaxValue);
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
            if (!IsPostBack)
            {
                ddlLog.DataSource = GetFeLogs();
                ddlLog.DataBind();
                ddlLog.Items.Insert(0, "Select A Log");
                ddlLimit.SelectedValue = "25";
            }
            PopulateLogs();
        }

        private void PopulateLogs()
        {
            if (ddlLog.Text != "Select A Log")
            {
                var limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
                gvLog.DataSource = GetLogContents(ddlLog.Text, limit);
                gvLog.DataBind();
            }
        }
    }
}