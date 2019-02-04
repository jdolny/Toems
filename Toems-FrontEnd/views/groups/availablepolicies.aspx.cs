using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.groups
{
    public partial class availablepolicies : BasePages.Groups
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.GroupPolicyUpdate);
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

            var listOfPolicies = Call.PolicyApi.Search(filter);
            gvPolicies.DataSource = listOfPolicies;
            gvPolicies.DataBind();

            lblTotal.Text = gvPolicies.Rows.Count + " Result(s) / " + Call.PolicyApi.GetCount() + " Policy(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<EntityPolicy>)gvPolicies.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "Trigger":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Trigger).ToList()
                        : listModules.OrderBy(h => h.Trigger).ToList();
                    break;
                case "Frequency":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Frequency).ToList()
                        : listModules.OrderBy(h => h.Frequency).ToList();
                    break;

            }

            gvPolicies.DataSource = listModules;
            gvPolicies.DataBind();
        }

    

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvPolicies);
        }

        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {
            var listOfGroupPolicies = new List<EntityGroupPolicy>();
            foreach (GridViewRow row in gvPolicies.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvPolicies.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var groupPolicy = new EntityGroupPolicy();
                groupPolicy.PolicyId = Convert.ToInt32(dataKey.Value);
                groupPolicy.GroupId = GroupEntity.Id;
                listOfGroupPolicies.Add(groupPolicy);
            }

            var result = Call.GroupPolicyApi.PostList(listOfGroupPolicies);
            EndUserMessage = result.Success ? "Successfully Added Policies To The Group" : result.ErrorMessage;
        }
    }
}