using System;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin.wolrelays
{
    public partial class search : Admin
    {
        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            foreach (GridViewRow row in gvRelays.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvRelays.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.WolRelayApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " Relays";
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvRelays);
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
            gvRelays.DataSource = Call.WolRelayApi.Search(filter);
            gvRelays.DataBind();

            lblTotal.Text = gvRelays.Rows.Count + " Result(s) / " + Call.WolRelayApi.GetCount() +
                            " WOL Relay(s)";
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