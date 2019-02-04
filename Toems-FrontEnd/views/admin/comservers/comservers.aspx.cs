using System;
using System.Web.UI.WebControls;
using Toems_Common;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class comservers1 : BasePages.Admin
    {
        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            foreach (GridViewRow row in gvServers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvServers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.ClientComServerApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " Servers";
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvServers);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            gvServers.DataSource = Call.ClientComServerApi.Get();
            gvServers.DataBind();

            lblTotal.Text = gvServers.Rows.Count + " Result(s) / " + Call.ClientComServerApi.GetCount() +
                            " Server(s)";
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