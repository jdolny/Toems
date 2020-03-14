using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.pxeboot
{
    public partial class createbootentry : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.PxeSettingsUpdate);
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            var order = 0;
            if (!int.TryParse(txtOrder.Text, out order))
            {
                EndUserMessage = "Order Not Valid";
                return;
            }
            var bootEntry = new EntityCustomBootMenu()
            {
                Name = txtName.Text,
                Description = txtDescription.Text,
                Content = txtContents.Text,
                Type = ddlType.Text,
                Order = order,
                IsActive = chkActive.Checked,
                IsDefault = chkDefault.Checked
            };

            var result = Call.CustomBootMenuApi.Post(bootEntry);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Added Boot Menu Entry";
                Response.Redirect("~/views/admin/pxeboot/editbootentry.aspx?&entryId=" + result.Id + "&level=2");
            }
        }
    }
}