using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.toec
{
    public partial class deployjobstatus : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateDeployJobs(ddlJobs);
                ddlJobs.Items.Insert(0, new ListItem("Select A Deploy Job", "-1"));
            }

        }

        protected void buttonRestart_Click(object sender, EventArgs e)
        {
            var result = Call.ToecDeployJobApi.RestartDeployJobService();
            if (result)
            {
                EndUserMessage = "Successfully Restarted Service";
            }
            else
                EndUserMessage = "Could Not Restart Service";
        }

        protected void ddlJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvComputers.DataSource = Call.ToecDeployJobApi.GetTargetComputers(Convert.ToInt32(ddlJobs.SelectedValue));
            gvComputers.DataBind();
        }

        protected void buttonResetStatus_Click(object sender, EventArgs e)
        {
            var count = 0;
            foreach (GridViewRow row in gvComputers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvComputers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.ToecDeployJobApi.ResetComputerStatus(Convert.ToInt32(dataKey.Value))) count++;
            }
            EndUserMessage = "Successfully Reset " + count + " Computers";
        }
    }
}