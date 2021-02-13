using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.groups
{
    public partial class search : BasePages.Groups
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateGrid();
                PopulateCategories();
                chkLdapGroups.Checked = false;
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
            filter.IncludeOus = chkLdapGroups.Checked;
            var listOfGroups = Call.GroupApi.Search(filter);
            gvGroups.DataSource = listOfGroups;
            gvGroups.DataBind();

            lblTotal.Text = gvGroups.Rows.Count + " Result(s) / " + Call.GroupApi.GetCount() + " Group(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<DtoGroupWithCount>)gvGroups.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "Type":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Type).ToList()
                        : listModules.OrderBy(h => h.Type).ToList();
                    break;
                case "MemberCount":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.MemberCount).ToList()
                        : listModules.OrderBy(h => h.MemberCount).ToList();
                    break;

            }

            gvGroups.DataSource = listModules;
            gvGroups.DataBind();
        }

        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;

            foreach (GridViewRow row in gvGroups.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvGroups.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.GroupApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    deletedCount++;
            }
            EndUserMessage = "Deleted " + deletedCount + " Group(s)";
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvGroups);
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

            PopulateGrid();
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

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
           DisplayConfirm();
        }

        protected void chkLdapGroups_CheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}