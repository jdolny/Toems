using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.groups
{
    public partial class assignedpolicies : BasePages.Groups
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.GroupPolicyRead);
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
            var filter = new DtoSearchFilter();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = txtSearch.Text;

            var modules = Call.GroupApi.GetAssignedPolicies(GroupEntity.Id, filter);
            gvPolicies.DataSource = modules;
            gvPolicies.DataBind();

        
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<GroupPolicyDetailed>)gvPolicies.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Policy.Name).ToList()
                        : listModules.OrderBy(h => h.Policy.Name).ToList();
                    break;
                case "PolicyOrder":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.PolicyOrder).ToList()
                        : listModules.OrderBy(h => h.PolicyOrder).ToList();
                    break;
                case "Trigger":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Policy.Trigger).ToList()
                        : listModules.OrderBy(h => h.Policy.Trigger).ToList();
                    break;
                case "Frequency":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Policy.Frequency).ToList()
                        : listModules.OrderBy(h => h.Policy.Frequency).ToList();
                    break;

            }

            gvPolicies.DataSource = listModules;
            gvPolicies.DataBind();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvPolicies);
        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.GroupPolicyUpdate);
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvPolicies.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var name = gvRow.Cells[2].Text;
                    var groupPolicy = Call.GroupPolicyApi.Get(Convert.ToInt32(dataKey.Value));
                    int orderValue;
                    if (!int.TryParse(((TextBox)gvRow.FindControl("txtOrder")).Text, out orderValue))
                        groupPolicy.PolicyOrder = 0;
                    else
                        groupPolicy.PolicyOrder = orderValue;
                    var result = Call.GroupPolicyApi.Put(groupPolicy.Id, groupPolicy);
                    if (result.Success) EndUserMessage = "Successfully Updated " + name;
                    else EndUserMessage = result.ErrorMessage;
                    PopulateGrid();
                }
            }

        }

        protected void btnRemove_OnClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvPolicies.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var name = gvRow.Cells[2].Text;
                    var result = Call.GroupPolicyApi.Delete(Convert.ToInt32(dataKey.Value));
                    if (result.Success) EndUserMessage = "Successfully Removed " + name;
                    else EndUserMessage = result.ErrorMessage;
                    PopulateGrid();
                }
            }
        }
    }
}