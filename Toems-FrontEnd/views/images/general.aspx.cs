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

        private void PopulateForm()
        {
            chkEnabled.Checked = ImageEntity.Enabled;
            chkProtected.Checked = ImageEntity.Protected;
            chkVisible.Checked = ImageEntity.IsVisible;
            txtImageName.Text = ImageEntity.Name;
            txtImageDesc.Text = ImageEntity.Description;
            ddlImageType.Text = ImageEntity.Type;
            ddlEnvironment.Text = ImageEntity.Environment;

            if (ImageEntity.Environment == "winpe")
            {
                imageType.Visible = false;
            }

            if (ImageEntity.Protected)
                chkProtected.Checked = true;

            //Image types can't be changed after they are created
            ddlImageType.Enabled = false;
            ddlEnvironment.Enabled = false;
        }
    }
}