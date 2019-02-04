using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.global.categories
{
    public partial class search : BasePages.Global
    {
        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            foreach (GridViewRow row in gvCategories.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvCategories.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.CategoryApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " Category(s)";
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvCategories);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilter();
            filter.SearchText = txtSearch.Text;
            filter.Limit = 0;
            gvCategories.DataSource = Call.CategoryApi.Search(filter);
            gvCategories.DataBind();

            lblTotal.Text = gvCategories.Rows.Count + " Result(s) / " + Call.CategoryApi.GetCount() +
                            " Category(s)";
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