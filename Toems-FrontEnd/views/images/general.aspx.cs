using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.images
{
    public partial class general : BasePages.Images
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ImageEntity.Name = txtImageName.Text;
            ImageEntity.Description = txtImageDesc.Text;
            ImageEntity.Enabled = chkEnabled.Checked;
            ImageEntity.Protected = chkProtected.Checked;
            ImageEntity.IsVisible = chkVisible.Checked;

            var result = Call.ImageApi.Put(ImageEntity.Id, ImageEntity);
            EndUserMessage = result.Success ? "Successfully Updated Image" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            chkEnabled.Checked = ImageEntity.Enabled;
            chkProtected.Checked = ImageEntity.Protected;
            chkVisible.Checked = ImageEntity.IsVisible;
            txtImageName.Text = ImageEntity.Name;
            txtImageDesc.Text = ImageEntity.Description;
            ddlEnvironment.Text = ImageEntity.Environment;

            ImageTypeSwitch();
            if (ddlEnvironment.Text == "winpe")
            {
                ddlImageTypeWinPe.Text = ImageEntity.Type;
                ddlImageTypeWinPe.Enabled = false;
            }
            else
            {
                ddlImageTypeLinux.Text = ImageEntity.Type;
                ddlImageTypeLinux.Enabled = false;
            }

                if (ImageEntity.Protected)
                chkProtected.Checked = true;



            ddlEnvironment.Enabled = false;
        }

        private void ImageTypeSwitch()
        {
            if (ddlEnvironment.Text == "winpe")
            {
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
    }
}