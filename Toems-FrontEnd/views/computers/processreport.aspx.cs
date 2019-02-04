using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.computers
{
    public partial class processreport : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void BindGrid()
        {
            var days = Convert.ToInt16(ddlDays.Text);
            var limit = Convert.ToInt16(ddlLimit.Text);

            if (ddlType.Text.Equals("Time"))
            {
                divTime.Visible = true;
                divCount.Visible = false;
                divUser.Visible = false;
                gvProcessTime.DataSource = Call.ComputerApi.ComputerProcessTimes(DateTime.UtcNow - TimeSpan.FromDays(days), limit, ComputerEntity.Id);
                gvProcessTime.DataBind();
            }
            else if (ddlType.Text.Equals("Count"))
            {
                divTime.Visible = false;
                divCount.Visible = true;
                divUser.Visible = false;
                gvProcessCount.DataSource = Call.ComputerApi.ComputerProcessCounts(DateTime.UtcNow - TimeSpan.FromDays(days), limit, ComputerEntity.Id);
                gvProcessCount.DataBind();
            }
            else if (ddlType.Text.Equals("User"))
            {
                divTime.Visible = false;
                divCount.Visible = false;
                divUser.Visible = true;
                gvProcessUser.DataSource = Call.ComputerApi.ComputerProcess(DateTime.UtcNow - TimeSpan.FromDays(days), limit, ComputerEntity.Id);
                gvProcessUser.DataBind();
            }
        }

        protected void btnRun_OnClick(object sender, EventArgs e)
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
            BindGrid();
        }

        protected void buttonExport_OnClick(object sender, EventArgs e)
        {
            if (ddlType.Text.Equals("Time"))
            {
                ExportToCSV(gvProcessTime, ComputerEntity.Name + "_ProcessTime.csv");
            }
            else if(ddlType.Text.Equals("Count"))
            {
                ExportToCSV(gvProcessCount, ComputerEntity.Name + "_ProcessCount.csv");
            }
            else
            {
                ExportToCSV(gvProcessUser, ComputerEntity.Name + "_ProcessUser.csv");
            }
        }
    }
}