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
    public partial class newmembers : BasePages.Assets
    {
        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {

            var memberships = (from GridViewRow row in gvAssets.Rows
                               let cb = (CheckBox)row.FindControl("chkSelector")
                               where cb != null && cb.Checked
                               select gvAssets.DataKeys[row.RowIndex]
                into dataKey
                               where dataKey != null
                               select new EntityAssetGroupMember()
                               {
                                   AssetId = Convert.ToInt32(dataKey.Value),
                                   AssetGroupId = AssetGroup.Id
                               }).ToList();
            EndUserMessage = Call.AssetGroupMemberApi.Post(memberships).Success
                ? "Successfully Added Group Members"
                : "Could Not Add Group Members";
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvAssets);
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateAssetTypes(ddlAssetType);
                ddlAssetType.Items.Insert(0, new ListItem("", ""));
                ddlAssetType.Items.Insert(1, new ListItem("Any Asset Type", "Any Asset Type"));
                ddlAssetType.SelectedIndex = 1;
                PopulateGrid();
                PopulateCategories();
            }
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilterCategories();
            filter.Limit = 0;
            filter.SearchText = txtSearch.Text;
            filter.Categories = SelectedCategories();
            filter.AssetType = ddlAssetType.SelectedItem.Text;
            filter.CategoryType = ddlCatType.Text;
            gvAssets.DataSource = Call.AssetApi.Search(filter);
            gvAssets.DataBind();

            lblTotal.Text = gvAssets.Rows.Count + " Result(s) / " + Call.AssetApi.GetCount() +
                            " Asset(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
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

        protected void ddlAssetType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}