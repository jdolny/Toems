using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.scriptmodules
{
    public partial class search : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateGrid();
                PopulateCategories();
            }
        }

        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void PopulateCategories()
        {
            selectCategory.DataSource = Call.CategoryApi.Get().Select(x => x.Name).ToList();
            selectCategory.DataBind();
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilterCategories();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = txtSearch.Text;
            filter.CategoryType = ddlCatType.Text;
            filter.Categories = SelectedCategories();

            var listOfModules = Call.ScriptModuleApi.Search(filter);
            gvModules.DataSource = listOfModules;
            gvModules.DataBind();

            lblTotal.Text = gvModules.Rows.Count + " Result(s) / " + Call.ScriptModuleApi.GetCount() + " Module(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<EntityScriptModule>)gvModules.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "ScriptType":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.ScriptType).ToList()
                        : listModules.OrderBy(h => h.ScriptType).ToList();
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
                        if (Call.ScriptModuleApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
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
                        if (Call.ModuleApi.Archive(Convert.ToInt32(dataKey.Value), EnumModule.ModuleType.Script).Success)
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

        protected void btnArchive_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Archive Selected Modules?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete Selected Modules?";
            Session["action"] = "delete";
            DisplayConfirm();
        }

        protected void ddlCatType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCatType.Text != "Any Category")
            {
                selectCategory.Visible = true;
                CategorySubmit.Visible = true;
            }
            else
            {
                selectCategory.Visible = false;
                CategorySubmit.Visible = false;
            }

        }

        protected void CategorySubmit_OnClick(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private List<string> SelectedCategories()
        {
            var list = new List<string>();
            foreach (ListItem a in selectCategory.Items)
            {

                if (a.Selected)
                    list.Add(a.Value);
            }

            return list;
        }
    }
}