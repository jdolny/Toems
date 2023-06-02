using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
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

            if(ddlEnvironment.Text == "winpe")
                image.Type = ddlImageTypeWinPe.Text;
            else
                image.Type = ddlImageTypeLinux.Text;

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

        private void ImageTypeSwitch()
        {
            if (ddlEnvironment.Text == "winpe")
            {
                var imageType = GetSetting(SettingStrings.DefaultWieImageType);
                if (!string.IsNullOrEmpty(imageType))
                    ddlImageTypeWinPe.Text = imageType;
                imageTypeWinPe.Visible = true;
                imageTypeLinux.Visible = false;
            }
            else
            {
                imageTypeWinPe.Visible = false;
                imageTypeLinux.Visible = true;
            }
        }
        protected void ddlEnvironment_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ImageTypeSwitch();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            ImageTypeSwitch();

            chkVisible.Checked = true;
        }
    }
}
