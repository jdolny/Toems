using System;
using Toems_Common;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class ResetPass : Users
    {
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var updatedUser = Call.ToemsUserApi.GetSelf();
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

            updatedUser.Email = txtEmail.Text;
            updatedUser.Theme = ddlTheme.Text;

            var result = Call.ToemsUserApi.ChangePassword(updatedUser);
            EndUserMessage = !result.Success ? result.ErrorMessage : "Successfully Updated User";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (ToemsCurrentUser.Id.ToString() != (string) Session["UserId"])
                Response.Redirect("~/views/dashboard/dash.aspx?access=denied");

            PopulateForm();
        }

        private void PopulateForm()
        {
            ddlTheme.DataSource = GetThemes();
            ddlTheme.DataBind();

            var user = Call.ToemsUserApi.GetSelf();
            if (user.IsLdapUser == 1)
            {
                lblLdap.Text = "TRUE";
                passwords.Visible = false;
            }
            else
            {
                lblLdap.Text = "FALSE";
            }
            txtEmail.Text = user.Email;
            ddlTheme.Text = user.Theme;
            
        }
    }
}