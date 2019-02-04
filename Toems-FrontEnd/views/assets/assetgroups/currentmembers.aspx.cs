using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.assets.assetgroups
{
    public partial class currentmembers : BasePages.Assets
    {


        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvAssets);
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            gvAssets.DataSource = Call.AssetGroupApi.GetGroupMembers(AssetGroup.Id);
            gvAssets.DataBind();
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            var removedCount = 0;
            foreach (GridViewRow row in gvAssets.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvAssets.DataKeys[row.RowIndex];
                if (dataKey != null)
                {
                    if (Call.AssetGroupApi.RemoveGroupMember(AssetGroup.Id, Convert.ToInt32(dataKey.Value)))
                        removedCount++;
                }
            }

            EndUserMessage = "Successfully Removed " + removedCount + " Members";

            PopulateGrid();
        }
    }
}