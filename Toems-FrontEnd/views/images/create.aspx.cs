using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.images
{
    public partial class create : BasePages.Images
    {
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var image = new EntityImage
            {
                Name = txtImageName.Text,
                Environment = ddlEnvironment.Text,
                Description = txtImageDesc.Text,
                Protected = chkProtected.Checked,
                IsVisible = chkVisible.Checked,
                Enabled = true,
            };

            image.Type = ddlEnvironment.Text == "winpe" ? "File" : ddlImageType.Text;

            var result = Call.ImageApi.Post(image);
            if (result.Success)
            {
                EndUserMessage = "Successfully Added Image";
                Response.Redirect("~/views/images/general.aspx?imageId=" + result.Id);
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }

        protected void ddlEnvironment_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEnvironment.Text == "winpe")
            {
                imageType.Visible = false;
            }
            else
            {
                imageType.Visible = true;
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;


            chkVisible.Checked = true;
        }
    }
}
