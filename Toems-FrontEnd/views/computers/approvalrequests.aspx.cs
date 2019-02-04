using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.computers
{
    public partial class approvalrequests : BasePages.Computers
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
            var filter = new DtoSearchFilter();
            filter.Limit = 0;
            filter.SearchText = txtSearch.Text;
            var listOfComputers = Call.ApprovalRequestApi.Search(filter);
            gvComputers.DataSource = listOfComputers;
            gvComputers.DataBind();

            lblTotal.Text = gvComputers.Rows.Count + " Result(s) / " + Call.ApprovalRequestApi.GetCount() + " Approval Request(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<EntityApprovalRequest>)gvComputers.DataSource;
            switch (e.SortExpression)
            {
                case "ComputerName":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.ComputerName).ToList()
                        : listModules.OrderBy(h => h.ComputerName).ToList();
                    break;
                case "IpAddress":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.IpAddress).ToList()
                        : listModules.OrderBy(h => h.IpAddress).ToList();
                    break;
                case "RequestTime":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.RequestTime).ToList()
                        : listModules.OrderBy(h => h.RequestTime).ToList();
                    break;

            }

            gvComputers.DataSource = listModules;
            gvComputers.DataBind();
        }

        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var action = (string)Session["action"];
            Session.Remove("action");
            var count = 0;
            switch (action)
            {
                case "deny":
                    foreach (GridViewRow row in gvComputers.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvComputers.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.ApprovalRequestApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Denied " + count + " Reset Request(s)";
                    break;
                case "approve":
                    foreach (GridViewRow row in gvComputers.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvComputers.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.ApprovalRequestApi.Approve(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Approved " + count + " Provision Request(s)";
                    break;
            }
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvComputers);
        }

        protected void btnApprove_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Approve Selected Approval Requests?";
            Session["action"] = "approve";
            DisplayConfirm();
        }

        protected void btnDeny_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Deny Selected Approval Requests?";
            Session["action"] = "deny";
            DisplayConfirm();
        }
    }
}