using System;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.global.schedules
{
    public partial class search : Global
    {
        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            foreach (GridViewRow row in gvSchedules.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvSchedules.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.ScheduleApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " Schedules";
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvSchedules);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilter();
            filter.Limit = 0;
            filter.SearchText = txtSearch.Text;
            gvSchedules.DataSource = Call.ScheduleApi.Search(filter);
            gvSchedules.DataBind();

            lblTotal.Text = gvSchedules.Rows.Count + " Result(s) / " + Call.ScheduleApi.GetCount() +
                            " Schedule(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
           DisplayConfirm();
        }
    }
}