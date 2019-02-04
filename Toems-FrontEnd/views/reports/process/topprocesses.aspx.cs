using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.reports.process
{
    public partial class toptentime : BasePages.Report
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
                gvProcessTime.DataSource = Call.ReportApi.TopProcessTimes(DateTime.UtcNow - TimeSpan.FromDays(days),limit);
                gvProcessTime.DataBind();
            }
            else if (ddlType.Text.Equals("Count"))
            {
                divTime.Visible = false;
                divCount.Visible = true;
                gvProcessCount.DataSource = Call.ReportApi.TopProcessCounts(DateTime.UtcNow - TimeSpan.FromDays(days), limit);
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
    }
}