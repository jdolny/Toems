using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace Toems_FrontEnd.views.policies
{
    public partial class search : BasePages.Policies
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
            var action = (string)Session["action"];
            Session.Remove("action");
            var count = 0;
            switch (action)
            {
                case "delete":
                    foreach (GridViewRow row in gvPolicies.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvPolicies.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.PolicyApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Deleted " + count + " Policy(s)";
                    break;
                case "archive":
                    foreach (GridViewRow row in gvPolicies.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvPolicies.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.PolicyApi.Archive(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Archived " + count + " Policy(s)";
                    break;
               
                   
            }

            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvPolicies);
        }

        protected void btnClone_OnClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvPolicies.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var name = gvRow.Cells[3].Text;
                    var result = Call.PolicyApi.Clone(Convert.ToInt32(dataKey.Value));
                    if (result.Success) EndUserMessage = "Successfully Cloned " + name;
                    else EndUserMessage = result.ErrorMessage;
                    PopulateGrid();
                }
            }

        }

        protected void btnActive_OnClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvPolicies.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var policy = Call.PolicyApi.Get(Convert.ToInt32(dataKey.Value));
                    if (policy.Frequency == EnumPolicy.Frequency.Ongoing)
                    {
                        var result = Call.PolicyApi.ActivatePolicy(policy.Id, false);
                        if (result.Success) EndUserMessage = "Successfully Activated " + policy.Name;
                        else EndUserMessage = result.ErrorMessage;
                        PopulateGrid();
                    }
                    else
                    {
                        var policyChanged = Call.PolicyApi.PolicyChangedSinceActivation(policy.Id);
                        if (policyChanged == "new" || policyChanged == "false")
                        {
                            var result = Call.PolicyApi.ActivatePolicy(policy.Id, false);
                            if (result.Success) EndUserMessage = "Successfully Activated " + policy.Name;
                            else EndUserMessage = result.ErrorMessage;
                            PopulateGrid();
                        }
                        else
                        {
                            Session["action"] = "activate";
                            Session["PolicyId"] = dataKey.Value;
                            Page.ClientScript.RegisterStartupScript(GetType(), "modalscript",
                                "$(function() {  var menuTop = document.getElementById('confirmbox2'),body = document.body;classie.toggle(menuTop, 'confirm-box-outer-open'); });",
                                true);
                        }
                    }
                }
            }
        }

        protected void btnDeactive_OnClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvPolicies.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var name = gvRow.Cells[3].Text;
                    var result = Call.PolicyApi.DeactivatePolicy(Convert.ToInt32(dataKey.Value));
                    if (result.Success) EndUserMessage = "Successfully Deactivated " + name;
                    else EndUserMessage = result.ErrorMessage;
                    PopulateGrid();
                }
            }
        }

        protected void gvPolicies_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var key = gvPolicies.DataKeys[e.Row.RowIndex];
                if (key != null)
                {
                    var active = e.Row.FindControl("lblActive") as Label;
                    if (Call.PolicyApi.GetActiveStatus(Convert.ToInt32(key.Value)) != null)
                    {
                     
                        e.Row.CssClass = "active_policy";
                        if (active != null) active.Text = "True";
                    }
                    else
                    {
                        if (active != null) active.Text = "False";
                    }
                        
                }

              
            }
        }

        protected void btnArchive_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Archive Selected Policies?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete Selected Policies?";
            Session["action"] = "delete";
            DisplayConfirm();
        }

        protected void btnAll_OnClick(object sender, EventArgs e)
        {
            var policyId = (int)Session["PolicyId"];
            Session.Remove("PolicyId");
            var policy = Call.PolicyApi.Get(policyId);
            var result = Call.PolicyApi.ActivatePolicy(policy.Id, true);
            if (result.Success) EndUserMessage = "Successfully Activated " + policy.Name;
            else EndUserMessage = result.ErrorMessage;
            PopulateGrid();
            
        }

        protected void btnNewOnly_OnClick(object sender, EventArgs e)
        {
            var policyId = (int)Session["PolicyId"];
            Session.Remove("PolicyId");
            var policy = Call.PolicyApi.Get(policyId);
            var result = Call.PolicyApi.ActivatePolicy(policy.Id, false);
            if (result.Success) EndUserMessage = "Successfully Activated " + policy.Name;
            else EndUserMessage = result.ErrorMessage;
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
    }
}