using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.reports.user
{
    public partial class processhistory : BasePages.Report
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void BindGrid()
        {
            if (ddlType.SelectedIndex == 0)
            {
                EndUserMessage = "You Must Select A Report Type";
                return;
            }
            if (ddlLimit.SelectedIndex == 0)
            {
                EndUserMessage = "You Must Select A Limit";
                return;
            }
            if (ddlDays.SelectedIndex == 0)
            {
                EndUserMessage = "You Must Select A Time Span";
                return;
            }
            var days = Convert.ToInt16(ddlDays.Text);
            var limit = Convert.ToInt16(ddlLimit.Text);

            if (ddlType.Text.Equals("Time"))
            {
                divTime.Visible = true;
                divCount.Visible = false;
                divUser.Visible = false;
                gvProcessTime.DataSource = Call.ToemsUserApi.UserProcessTimes(DateTime.UtcNow - TimeSpan.FromDays(days), limit, txtUser.Text);
                gvProcessTime.DataBind();
            }
            else if (ddlType.Text.Equals("Count"))
            {
                divTime.Visible = false;
                divCount.Visible = true;
                divUser.Visible = false;
                gvProcessCount.DataSource = Call.ToemsUserApi.UserProcessCounts(DateTime.UtcNow - TimeSpan.FromDays(days), limit, txtUser.Text);
                gvProcessCount.DataBind();
            }
            else if (ddlType.Text.Equals("User"))
            {
                divTime.Visible = false;
                divCount.Visible = false;
                divUser.Visible = true;
                gvProcessUser.DataSource = Call.ToemsUserApi.UserProcess(DateTime.UtcNow - TimeSpan.FromDays(days), limit, txtUser.Text);
                gvProcessUser.DataBind();
            }
        }

        protected void btnRun_OnClick(object sender, EventArgs e)
        {
           
            BindGrid();
        }

        protected void buttonExport_OnClick(object sender, EventArgs e)
        {
            if (ddlType.Text.Equals("Time"))
            {
                ExportToCSV(gvProcessTime, txtUser.Text + "_ProcessTime.csv");
            }
            else if (ddlType.Text.Equals("Count"))
            {
                ExportToCSV(gvProcessCount, txtUser.Text + "_ProcessCount.csv");
            }
            else
            {
                ExportToCSV(gvProcessUser, txtUser.Text + "_ProcessUser.csv");
            }
        }

        protected void txtUser_OnTextChanged(object sender, EventArgs e)
        {
           BindGrid();
        }
    }
}