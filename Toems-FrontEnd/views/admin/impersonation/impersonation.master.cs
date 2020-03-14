using System;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin.impersonation
{
    public partial class impersonation : BasePages.MasterBaseMaster
    {
        public EntityImpersonationAccount ImpAccount { get; set; }
        private BasePages.Admin AdminBasePage { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            AdminBasePage = Page as BasePages.Admin;
            AdminBasePage.RequiresAuthorization(AuthorizationStrings.Administrator);
            ImpAccount = AdminBasePage.ImpersonationAccount;

            if (ImpAccount == null)
            {
                divLevel3.Visible = false;
                btnDelete.Visible = false;
            }
            else
            {
                divLevel2.Visible = false;
                btnDelete.Visible = true;
            }
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + ImpAccount.Username + "?";
            DisplayConfirm();
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            var result = new DtoActionResult();
            result = new APICall().ImpersonationAccountApi.Delete(ImpAccount.Id);

            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully Deleted Impersonation Account: " + ImpAccount.Username;
                Response.Redirect("~/views/admin/impersonation/search.aspx?level=2");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }


    }
}