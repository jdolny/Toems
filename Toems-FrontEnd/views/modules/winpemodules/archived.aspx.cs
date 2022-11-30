using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.winpemodules
{
    public partial class archived : BasePages.Modules
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

            var listOfModules = Call.WinPeModuleApi.GetArchived(filter);
            gvModules.DataSource = listOfModules;
            gvModules.DataBind();

            lblTotal.Text = gvModules.Rows.Count + " Result(s) / " + Call.WinPeModuleApi.GetArchivedCount() + " Module(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<EntityWinPeModule>)gvModules.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;

            }

            gvModules.DataSource = listModules;
            gvModules.DataBind();
        }

        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var action = (string)Session["action"];
            Session.Remove("action");
            var count = 0;
            switch (action)
            {
                case "delete":
                    foreach (GridViewRow row in gvModules.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvModules.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.WinPeModuleApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Deleted " + count + " Module(s)";
                    break;
                case "archive":
                    foreach (GridViewRow row in gvModules.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvModules.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.ModuleApi.Archive(Convert.ToInt32(dataKey.Value), EnumModule.ModuleType.WinPE).Success)
                            count++;
                    }
                    EndUserMessage = "Archived " + count + " Module(s)";
                    break;
            }
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvModules);
        }



        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete Selected Modules?";
            Session["action"] = "delete";
            DisplayConfirm();
        }

        protected void btnRestore_OnClick(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvModules.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var result = Call.ModuleApi.Restore(Convert.ToInt32(dataKey.Value), EnumModule.ModuleType.WinPE);
                    EndUserMessage = result.Success ? "Successfully Restored Module" : result.ErrorMessage;
                    PopulateGrid();
                }
            }
        }
    }
}