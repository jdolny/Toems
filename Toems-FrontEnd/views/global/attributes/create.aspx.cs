using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.global.attributes
{
    public partial class create : BasePages.Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateTextMode(ddlTextMode);
            }
        }

        protected void buttonAdd_OnClick(object sender, EventArgs e)
        {
            var ca = new EntityCustomAttribute();
            ca.Name = txtName.Text;
            ca.Description = txtDescription.Text;
            ca.TextMode = (EnumCustomAttribute.TextMode)Enum.Parse(typeof(EnumCustomAttribute.TextMode), ddlTextMode.SelectedValue);
            ca.UsageType = -3;
            ca.ClientImagingAvailable = chkImaging.Checked;
            var result = Call.CustomAttributeApi.Post(ca);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Custom Attribute";
                Response.Redirect("~/views/global/attributes/edit.aspx?level=2&attributeId=" + result.Id);
            }
        }
    }
}