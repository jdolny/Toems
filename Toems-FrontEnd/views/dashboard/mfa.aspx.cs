using System;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.dashboard
{
    public partial class Mfa : Users
    {
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var result = Call.ToemsUserApi.VerifyMfaSecret(txtVerify.Text);
            EndUserMessage = result ? "Successfully Updated User MFA.  You must logout and login again." : "Could Not Verify Code.";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateForm();
        }

        private void PopulateForm()
        {
            q.Src = Call.ToemsUserApi.GenerateTempMfaSecret();
        }
    }
}