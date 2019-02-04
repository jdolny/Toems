using System;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_FrontEnd.BasePages;

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

            //Don't remove all administrators
            if (ToemsUserGroup.Membership == "User")
            {
                var existingAdminCount = Call.ToemsUserApi.GetAdminCount();
                var selectedAdminCount = 0;
                foreach (GridViewRow row in gvUsers.Rows)
                {
                    var cb = (CheckBox) row.FindControl("chkSelector");
                    if (cb == null || !cb.Checked) continue;
                    var dataKey = gvUsers.DataKeys[row.RowIndex];
                    if (dataKey != null)
                    {
                        var user = Call.ToemsUserApi.Get(Convert.ToInt32(dataKey.Value));
                        if (user.Membership == "Administrator")
                            selectedAdminCount++;
                    }
                }
                if (existingAdminCount == selectedAdminCount)
                {
                    EndUserMessage = "Cannot Move Users To Group.  It Would Remove All Administrators From The System";
                    return;
                }
            }

            var successCount = 0;
            foreach (GridViewRow row in gvUsers.Rows)
            {
                var cb = (CheckBox) row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvUsers.DataKeys[row.RowIndex];
                if (dataKey != null)
                {
                    var user = Call.ToemsUserApi.Get(Convert.ToInt32(dataKey.Value));

                    Call.UserGroupApi.AddNewMember(ToemsUserGroup.Id, user.Id);
                    successCount++;
                }
            }
            EndUserMessage += "Successfully Added " + successCount + " Users To The Group";
            PopulateGrid();
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