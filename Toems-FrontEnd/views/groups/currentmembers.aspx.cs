using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.groups
{
    public partial class currentmembers : BasePages.Groups
    {
        protected void btnRemoveSelected_OnClick(object sender, EventArgs e)
        {
            if (GroupEntity.Type == "Dynamic")
            {
                EndUserMessage = "Computers Cannot Be Removed From Dynamic Groups";
                return;
            }
            if (GroupEntity.IsOu)
            {
                EndUserMessage = "Computers Cannot Be Removed From Active Directory OU's";
                return;
            }
            if (GroupEntity.IsSecurityGroup)
            {
                EndUserMessage = "Computers Cannot Be Removed From Active Directory Security Groups";
                return;
            }

            var removedCount = 0;
            foreach (GridViewRow row in gvComputers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvComputers.DataKeys[row.RowIndex];
                if (dataKey != null)
                {
                    if (Call.GroupApi.RemoveGroupMember(GroupEntity.Id, Convert.ToInt32(dataKey.Value)))
                        removedCount++;
                }
            }

            EndUserMessage = "Successfully Removed " + removedCount + " Members";

            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvComputers);
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
            filter.Limit = 0;
            filter.SearchText = txtSearch.Text;
            gvComputers.DataSource = Call.GroupApi.GetGroupMembers(GroupEntity.Id, filter);
            gvComputers.DataBind();

            lblTotal.Text = gvComputers.Rows.Count + " Result(s) / " + Call.GroupApi.GetMemberCount(GroupEntity.Id) +
                            " Total Computer(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}