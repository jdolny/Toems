using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class UserSearch : Users
    {
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var deletedCount = 0;
            var adminMessage = string.Empty;
            foreach (GridViewRow row in gvUsers.Rows)
            {
                var cb = (CheckBox) row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvUsers.DataKeys[row.RowIndex];
                if (dataKey != null)
                {
                    var user = Call.ToemsUserApi.Get(Convert.ToInt32(dataKey.Value));

                    if (user.Membership == "Administrator")
                    {
                        adminMessage =
                            "<br/>Administrators Must Be Changed To The User Role Before They Can Be Deleted";
                        break;
                    }
                    if (Call.ToemsUserApi.Delete(user.Id).Success)
                        deletedCount++;
                }
            }
            EndUserMessage = "Successfully Deleted " + deletedCount + " User(s)" + adminMessage;
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvUsers);
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();
            var listUsers = (List<UserWithUserGroup>) gvUsers.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listUsers = GetSortDirection(e.SortExpression) == "Asc"
                        ? listUsers.OrderBy(g => g.Name).ToList()
                        : listUsers.OrderByDescending(g => g.Name).ToList();
                    break;
                case "Membership":
                    listUsers = GetSortDirection(e.SortExpression) == "Asc"
                        ? listUsers.OrderBy(g => g.Membership).ToList()
                        : listUsers.OrderByDescending(g => g.Membership).ToList();
                    break;
            }

            gvUsers.DataSource = listUsers;
            gvUsers.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ToemsCurrentUser.Membership != "Administrator")
            {
                Session["UserId"] = ToemsCurrentUser.Id.ToString();
                Response.Redirect("~/views/users/resetpass.aspx", true);
            }

            //Just In Case
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

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
           DisplayConfirm();
        }
    }
}