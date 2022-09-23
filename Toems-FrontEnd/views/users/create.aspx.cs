using System;
using Toems_Common;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class CreateUser : Users
    {

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!chkldap.Checked)
            {
                if (txtUserPwd.Text != txtUserPwdConfirm.Text)
                {
                    EndUserMessage = "Passwords Did Not Match";
                    return;
                }

                if (string.IsNullOrEmpty(txtUserPwd.Text))
                {
                    EndUserMessage = "Passwords Cannot Be Empty";
                    return;
                }

                if (txtUserPwd.Text.Length < 8)
                {
                    EndUserMessage = "Passwords Must Be At Least 8 Characters";
                    return;
                }
            }
            else
            {
                //Create a local random db pass, should never actually be possible to use anyway.
                txtUserPwd.Text = new Guid().ToString();
                txtUserPwdConfirm.Text = txtUserPwd.Text;
            }

            var user = new EntityToemsUser
            {
                Name = txtUserName.Text,
                Membership = ddluserMembership.Text,
                Salt = Utility.CreateSalt(64),
                Email = txtEmail.Text,
                Theme = ddlTheme.Text,
                IsLdapUser = chkldap.Checked ? 1 : 0,
                UserGroupId = -1,
                ComputerSortMode = ddlComputerSort.Text,
                DefaultLoginPage = ddlLoginPage.Text,
                DefaultComputerView = ddlComputerView.Text,
                EnableWebMfa = chkWebMfa.Checked,
                EnableImagingMfa = chkImagingMfa.Checked
        };

            user.Password = Utility.CreatePasswordHash(txtUserPwd.Text, user.Salt);
            var result = Call.ToemsUserApi.Post(user);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created User";
                Response.Redirect("~/views/users/edit.aspx?userid=" + result.Id);
            }
        }

        protected void chkldap_OnCheckedChanged(object sender, EventArgs e)
        {
            passwords.Visible = !chkldap.Checked;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (!IsPostBack)
            {
                ddlTheme.DataSource = GetThemes();
                ddlTheme.DataBind();
            }
        }
    }
}