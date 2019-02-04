using System;
using System.Web.UI.WebControls;
using Toems_Common;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class comservercluster : BasePages.Admin
    {
        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            foreach (GridViewRow row in gvClusters.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvClusters.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.ComServerClusterApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " Clusters";
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvClusters);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            gvClusters.DataSource = Call.ComServerClusterApi.Get();
            gvClusters.DataBind();

            lblTotal.Text = gvClusters.Rows.Count + " Result(s) / " + Call.ComServerClusterApi.GetCount() +
                            " Cluster(s)";
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