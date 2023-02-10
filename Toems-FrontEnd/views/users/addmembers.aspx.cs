using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;
using Toems_FrontEnd.views.computers;

namespace Toems_FrontEnd.views.users
{
    public partial class views_users_addmembers : Users
    {
        protected void btnAddSelected_OnClick(object sender, EventArgs e)
        {
            if (ToemsUserGroup.IsLdapGroup == 1)
            {
                EndUserMessage = "Users Cannot Be Added To LDAP Groups";
                return;
            }
            var memberships = (from GridViewRow row in gvUsers.Rows
                               let cb = (CheckBox)row.FindControl("chkSelector")
                               where cb != null && cb.Checked
                               select gvUsers.DataKeys[row.RowIndex]
               into dataKey
                               where dataKey != null
                               select new EntityUserGroupMembership()
                               {
                                   ToemsUserId = Convert.ToInt32(dataKey.Value),
                                   UserGroupId = ToemsUserGroup.Id
                               }).ToList();
            var result = Call.UserGroupMembershipApi.Post(memberships);
            EndUserMessage = result.Success
                ? "Successfully Added Group Members"
                : "Could Not Add Group Members." + result.ErrorMessage;
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvUsers);
        }

        protected void ddlLimit_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilter();
            filter.Limit = 0;
            filter.SearchText = txtSearch.Text;
            gvUsers.DataSource = Call.ToemsUserApi.Search(filter);
            gvUsers.DataBind();
            lblTotal.Text = gvUsers.Rows.Count + " Result(s) / " + Call.ToemsUserApi.GetCount() + " Total User(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}