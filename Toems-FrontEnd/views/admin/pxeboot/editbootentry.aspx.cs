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
    public partial class editbootentry : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.PxeSettingsUpdate);
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            txtName.Text = BootEntry.Name;
            txtDescription.Text = BootEntry.Description;
            txtContents.Text = BootEntry.Content;
            ddlType.Text = BootEntry.Type;
         
            txtOrder.Text = BootEntry.Order.ToString();
            chkActive.Checked = BootEntry.IsActive;
            chkDefault.Checked = BootEntry.IsDefault;
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {

            BootEntry.Name = txtName.Text;
            BootEntry.Description = txtDescription.Text;
            BootEntry.Content = txtContents.Text;
            BootEntry.Type = ddlType.Text;
            var order = 0;
            if (!int.TryParse(txtOrder.Text, out order))
            {
                EndUserMessage = "Order Not Valid";
                return;
            }
            BootEntry.Order = order;
            BootEntry.IsActive = chkActive.Checked;
            BootEntry.IsDefault = chkDefault.Checked;


            var result = Call.CustomBootMenuApi.Put(BootEntry.Id, BootEntry);
            EndUserMessage = !result.Success ? result.ErrorMessage : "Successfully Updated Boot Menu Entry";
        }
    }
}