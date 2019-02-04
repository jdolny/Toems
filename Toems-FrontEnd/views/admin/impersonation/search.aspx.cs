using System;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.admin.impersonation
{
    public partial class search : BasePages.Admin
    {
        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            foreach (GridViewRow row in gvAccounts.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvAccounts.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.ImpersonationAccountApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " Impersonation Accounts";
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvAccounts);
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

            gvAccounts.DataSource = Call.ImpersonationAccountApi.Search(filter);
            gvAccounts.DataBind();

            lblTotal.Text = gvAccounts.Rows.Count + " Result(s) / " + Call.ImpersonationAccountApi.GetCount() +
                            " Impersonation Account(s)";
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