using System;
using Toems_Common;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class UserMaster : MasterBaseMaster
    {
        public EntityToemsUser ToemsUser { get; set; }
        public EntityToemsUserGroup ToemsUserGroup { get; set; }
        private Users UsersBasePage { get; set; }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            UsersBasePage.RequiresAuthorization(AuthorizationStrings.Administrator);
            if (UsersBasePage.Call.ToemsUserApi.GetAdminCount() == 1 &&
                ToemsUser.Membership == "Administrator")
            {
                PageBaseMaster.EndUserMessage = "There Must Be At Least One Administrator";
            }
            else
            {
                lblTitle.Text = "Delete This User?";
                Page.ClientScript.RegisterStartupScript(GetType(), "modalscript",
                    "$(function() {  var menuTop = document.getElementById('confirmbox'),body = document.body;classie.toggle(menuTop, 'confirm-box-outer-open'); });",
                    true);
            }
        }

        protected void btnDeleteGroup_Click(object sender, EventArgs e)
        {
            UsersBasePage.RequiresAuthorization(AuthorizationStrings.Administrator);

            lblTitle.Text = "Delete This User Group?";
            Page.ClientScript.RegisterStartupScript(GetType(), "modalscript",
                "$(function() {  var menuTop = document.getElementById('confirmbox'),body = document.body;classie.toggle(menuTop, 'confirm-box-outer-open'); });",
                true);
        }

        protected void OkButton_Click(object sender, EventArgs e)
        {
            if (ToemsUser != null)
            {
                var result = UsersBasePage.Call.ToemsUserApi.Delete(ToemsUser.Id);
                if (result.Success)
                {
                    PageBaseMaster.EndUserMessage = "Successfully Deleted User";
                    Response.Redirect("~/views/users/search.aspx");
                }
                else
                {
                    PageBaseMaster.EndUserMessage = result.ErrorMessage;
                }
            }
            else if (ToemsUserGroup != null)
            {
                var result = UsersBasePage.Call.UserGroupApi.Delete(ToemsUserGroup.Id);
                if (result.Success)
                {
                    PageBaseMaster.EndUserMessage = "Successfully Deleted User Group";
                    Response.Redirect("~/views/users/searchgroup.aspx");
                }
                else
                {
                    PageBaseMaster.EndUserMessage = result.ErrorMessage;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UsersBasePage = Page as Users;
            ToemsUser = UsersBasePage.ToemsUser;
            ToemsUserGroup = UsersBasePage.ToemsUserGroup;
            if (ToemsUser == null && ToemsUserGroup == null) //level 1
            {
                Level2.Visible = false;
                Level2Group.Visible = false;
                Level3.Visible = false;
                btnDelete.Visible = false;
            }
            else
            {
                if (ToemsUser == null)
                {
                    Level2.Visible = false;
                    btnDelete.Visible = false;
                    btnDeleteGroup.Visible = true;
                }
                if (ToemsUserGroup == null)
                {
                    Level2Group.Visible = false;
                    btnDelete.Visible = true;
                    btnDeleteGroup.Visible = false;
                }

                Level1.Visible = false;
                if (Request.QueryString["level"] == "3")
                {
                    Level2.Visible = false;
                    Level2Group.Visible = false;
                }
            }

            if (UsersBasePage.ToemsCurrentUser.Membership == "User")
                Level1.Visible = false;
        }
    }
}