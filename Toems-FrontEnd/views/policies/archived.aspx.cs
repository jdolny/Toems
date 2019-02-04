using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.policies
{
    public partial class archived : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateGrid();
            }
        }

        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilterCategories();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = txtSearch.Text;

            var listOfPolicies = Call.PolicyApi.GetArchived(filter);
            gvPolicies.DataSource = listOfPolicies;
            gvPolicies.DataBind();

            lblTotal.Text = gvPolicies.Rows.Count + " Result(s) / " + Call.PolicyApi.GetArchivedCount() + " Policy(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<EntityPolicy>) gvPolicies.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "Frequency":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Frequency).ToList()
                        : listModules.OrderBy(h => h.Frequency).ToList();
                    break;
                case "Trigger":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Trigger).ToList()
                        : listModules.OrderBy(h => h.Trigger).ToList();
                    break;

            }

            gvPolicies.DataSource = listModules;
            gvPolicies.DataBind();
        }

        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var action = (string) Session["action"];
            Session.Remove("action");
            var count = 0;
            switch (action)
            {
                case "delete":
                    foreach (GridViewRow row in gvPolicies.Rows)
                    {
                        var cb = (CheckBox) row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvPolicies.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.PolicyApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Deleted " + count + " Policy(s)";
                    break;
            }

            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvPolicies);
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete Selected Policies?";
            Session["action"] = "delete";
            DisplayConfirm();
        }

        protected void btnRestore_OnClick(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                var gvRow = (GridViewRow) control.Parent.Parent;
                var dataKey = gvPolicies.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var result = Call.PolicyApi.Restore(Convert.ToInt32(dataKey.Value));
                    EndUserMessage = result.Success ? "Successfully Restored Policy" : result.ErrorMessage;
                    PopulateGrid();
                }
            }
        }
    }

}