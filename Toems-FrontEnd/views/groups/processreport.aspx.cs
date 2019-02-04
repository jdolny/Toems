using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.groups
{
    public partial class processreport : BasePages.Groups
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
                gvProcessTime.DataSource = Call.GroupApi.GroupProcessTimes(DateTime.UtcNow - TimeSpan.FromDays(days), limit,GroupEntity.Id);
                gvProcessTime.DataBind();
            }
            else if (ddlType.Text.Equals("Count"))
            {
                divTime.Visible = false;
                divCount.Visible = true;
                gvProcessCount.DataSource = Call.GroupApi.GroupProcessCounts(DateTime.UtcNow - TimeSpan.FromDays(days), limit,GroupEntity.Id);
                gvProcessCount.DataBind();
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
                ExportToCSV(gvProcessTime, GroupEntity.Name + "_ProcessTime.csv");
            }
            else
            {
                ExportToCSV(gvProcessCount, GroupEntity.Name + "_ProcessCount.csv");
            }
        }
    }
}