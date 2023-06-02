using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

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

        private void PopulateGrid()
        {
            gvComputers.DataSource = Call.ToecDeployJobApi.GetTargetComputers(Convert.ToInt32(ddlJobs.SelectedValue));
            gvComputers.DataBind();
        }
        protected void ddlJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
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

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listComputers = (List<EntityToecTargetListComputer>)gvComputers.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listComputers = GetSortDirection(e.SortExpression) == "Desc"
                        ? listComputers.OrderByDescending(h => h.Name).ToList()
                        : listComputers.OrderBy(h => h.Name).ToList();
                    break;
                case "Status":
                    listComputers = GetSortDirection(e.SortExpression) == "Desc"
                        ? listComputers.OrderByDescending(h => h.Status).ToList()
                        : listComputers.OrderBy(h => h.Status).ToList();
                    break;
                case "LastStatusDate":
                    listComputers = GetSortDirection(e.SortExpression) == "Desc"
                        ? listComputers.OrderByDescending(h => h.LastStatusDate).ToList()
                        : listComputers.OrderBy(h => h.LastStatusDate).ToList();
                    break;
                case "LastUpdateDetails":
                    listComputers = GetSortDirection(e.SortExpression) == "Desc"
                        ? listComputers.OrderByDescending(h => h.LastUpdateDetails).ToList()
                        : listComputers.OrderBy(h => h.LastUpdateDetails).ToList();
                    break;
              

            }

            gvComputers.DataSource = listComputers;
            gvComputers.DataBind();
        }
    }
}