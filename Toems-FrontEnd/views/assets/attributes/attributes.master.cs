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
        private BasePages.Assets AssetsBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AssetsBasePage = Page as BasePages.Assets;
            AssetsBasePage.RequiresAuthorization(AuthorizationStrings.CustomAttributeRead);
            CustomAttribute = AssetsBasePage.CustomAttribute;
            if (CustomAttribute == null)
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
            lblTitle.Text = "Delete " + CustomAttribute.Name + "? This Will Also Delete The Attribute From All Assets And Computers";
            DisplayConfirm();
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            var result = new DtoActionResult();
            result = AssetsBasePage.Call.CustomAttributeApi.Delete(CustomAttribute.Id);

            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully Deleted Custom Attribute: " + CustomAttribute.Name;
                Response.Redirect("~/views/assets/attributes/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }
    }
}