using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.assets.assetgroups
{
    public partial class search : BasePages.Assets
    {
        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            foreach (GridViewRow row in gvAssetGroups.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvAssetGroups.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.AssetGroupApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " Asset Group(s)";
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvAssetGroups);
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
            gvAssetGroups.DataSource = Call.AssetGroupApi.Search(filter);
            gvAssetGroups.DataBind();

            lblTotal.Text = gvAssetGroups.Rows.Count + " Result(s) / " + Call.AssetGroupApi.GetCount() +
                            " Asset Group(s)";
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