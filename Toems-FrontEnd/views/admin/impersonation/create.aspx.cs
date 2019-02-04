using System;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.impersonation
{
    public partial class create : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void buttonAddAccount_OnClick(object sender, EventArgs e)
        {
            var account = new EntityImpersonationAccount()
            {
                Username = txtUsername.Text,
                Password = txtPassword.Text,
            };

            var result = Call.ImpersonationAccountApi.Post(account);
            if (result.Success)
            {
                EndUserMessage = "Successfully Created Account";
                Response.Redirect("~/views/admin/impersonation/edit.aspx?level=2&impersonationId=" + result.Id);
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
}