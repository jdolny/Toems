using System;
using Toems_Common;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class EditUser : Users
    {

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Call.ToemsUserApi.GetAdminCount() == 1 && ddluserMembership.Text != "Administrator" &&
                ToemsUser.Membership == "Administrator")
            {
                EndUserMessage = "There Must Be At Least One Administrator";
                return;
            }

            if (ToemsUser.UserGroupId != -1 && ddluserMembership.Text != ToemsUser.Membership)
            {
                EndUserMessage = "This User's Role Is Controlled By A Group";
                return;
            }

            var updatedUser = ToemsUser;
            if (!string.IsNullOrEmpty(txtUserPwd.Text))
            {
                if (txtUserPwd.Text == txtUserPwdConfirm.Text)
                {
                    updatedUser.Salt = Utility.CreateSalt(64);
                    updatedUser.Password = Utility.CreatePasswordHash(txtUserPwd.Text, updatedUser.Salt);
                }
                else
                {
                    EndUserMessage = "Passwords Did Not Match";
                    return;
                }

                if (txtUserPwd.Text.Length < 8)
                {
                    EndUserMessage = "Passwords Must Be At Least 8 Characters";
                    return;
                }
            }

            updatedUser.Name = txtUserName.Text;
            updatedUser.Membership = ddluserMembership.Text;
            updatedUser.Email = txtEmail.Text;
            updatedUser.Theme = ddlTheme.Text;
            var result = Call.ToemsUserApi.Put(updatedUser.Id, updatedUser);
            EndUserMessage = !result.Success ? result.ErrorMessage : "Successfully Updated User";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            ddlTheme.DataSource = GetThemes();
            ddlTheme.DataBind();
            if (ToemsUser.IsLdapUser == 1)
            {
                lblLdap.Text = "TRUE";
                passwords.Visible = false;
            }
            else
            {
                lblLdap.Text = "FALSE";
            }
            txtUserName.Text = ToemsUser.Name;
            ddluserMembership.Text = ToemsUser.Membership;
            txtEmail.Text = ToemsUser.Email;
            ddlTheme.Text = ToemsUser.Theme;

        }
    }
}