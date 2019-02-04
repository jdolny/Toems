using System;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.impersonation
{
    public partial class edit : BasePages.Admin
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdateAccount_OnClick(object sender, EventArgs e)
        {
            ImpersonationAccount.Username = txtUsername.Text;
            ImpersonationAccount.Password = txtPassword.Text;

            var result = Call.ImpersonationAccountApi.Put(ImpersonationAccount.Id,ImpersonationAccount);
            EndUserMessage = result.Success ? "Successfully Updated Account" : result.ErrorMessage;

        }

        private void PopulateForm()
        {
            
            txtUsername.Text = ImpersonationAccount.Username;
        }
    }
}