using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.groups
{
    public partial class addmembers : BasePages.Groups
    {
        protected void btnAddSelected_OnClick(object sender, EventArgs e)
        {
            if(GroupEntity.Id == -1)
            {
                EndUserMessage = "Computers Cannot Be Added To The Built-In All Computers Group";
                return;
            }
            var memberships = (from GridViewRow row in gvComputers.Rows
                let cb = (CheckBox) row.FindControl("chkSelector")
                where cb != null && cb.Checked
                select gvComputers.DataKeys[row.RowIndex]
                into dataKey
                where dataKey != null
                select new EntityGroupMembership()
                {
                    ComputerId = Convert.ToInt32(dataKey.Value),
                    GroupId = GroupEntity.Id
                }).ToList();
            var result = Call.GroupMembershipApi.Post(memberships);
            EndUserMessage =result.Success
                ? "Successfully Added Group Members"
                : "Could Not Add Group Members." + result.ErrorMessage;
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvComputers);
        }

        protected void ddlLimit_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();
            var listComputers = (List<EntityComputer>)gvComputers.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listComputers = GetSortDirection(e.SortExpression) == "Asc"
                        ? listComputers.OrderBy(h => h.Name).ToList()
                        : listComputers.OrderByDescending(h => h.Name).ToList();
                    break;
            }

            gvComputers.DataSource = listComputers;
            gvComputers.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
        
                PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilter();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = txtSearch.Text;

            var listOfComputers = Call.ComputerApi.SearchForGroup(filter).ToList();

            gvComputers.DataSource = listOfComputers;
            gvComputers.DataBind();

            lblTotal.Text = gvComputers.Rows.Count + " Result(s) / " + Call.ComputerApi.GetCount() +
                            " Total Computer(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}