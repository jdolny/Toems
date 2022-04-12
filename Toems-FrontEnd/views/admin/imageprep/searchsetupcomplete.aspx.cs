using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.imageprep
{
    public partial class searchsetupcomplete : BasePages.Admin
    {
        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            foreach (GridViewRow row in gvEntries.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvEntries.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.SetupCompleteFileApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " Sysprep SetupComplete File(s)";
            PopulateGrid();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            gvEntries.DataSource = Call.SetupCompleteFileApi.Get();
            gvEntries.DataBind();
        }
        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            DisplayConfirm();
        }

        protected void chkSelectAll_CheckedChanged1(object sender, EventArgs e)
        {
            ChkAll(gvEntries);
        }
    }
}