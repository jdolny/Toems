using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.global.attributes
{
    public partial class attributes : BasePages.MasterBaseMaster
    {
        public EntityCustomAttribute CustomAttribute { get; set; }
        private BasePages.Global GlobalBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            GlobalBasePage = Page as BasePages.Global;
            GlobalBasePage.RequiresAuthorization(AuthorizationStrings.CustomAttributeRead);
            CustomAttribute = GlobalBasePage.CustomAttribute;
            if (CustomAttribute != null)
            {
                Level1.Visible = false;
                Level2.Visible = true;
                btnDelete.Visible = true;

            }
            else
            {
                Level1.Visible = true;
                Level2.Visible = false;
                btnDelete.Visible = false;
            }

        }
        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + CustomAttribute.Name + "? This Will Also Delete The Attribute From All Assets And Computers";
            DisplayConfirm();
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            var result = new DtoActionResult();
            result = GlobalBasePage.Call.CustomAttributeApi.Delete(CustomAttribute.Id);

            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully Deleted Custom Attribute: " + CustomAttribute.Name;
                Response.Redirect("~/views/global/attributes/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }
    }
}