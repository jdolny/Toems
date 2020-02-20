using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;

namespace Toems_FrontEnd.views.images.profiles
{
    public partial class pxe : BasePages.Images
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlKernel.DataSource = Call.FilesystemApi.GetKernels();
                ddlBootImage.DataSource = Call.FilesystemApi.GetBootImages();
                ddlKernel.DataBind();
                ddlBootImage.DataBind();
                ddlBootImage.SelectedValue = SettingStrings.DefaultInit;
                ddlKernel.Text = ImageProfile.Kernel;
                ddlBootImage.Text = ImageProfile.BootImage;
                txtKernelArgs.Text = ImageProfile.KernelArguments;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            ImageProfile.Kernel = ddlKernel.Text;
            ImageProfile.BootImage = ddlBootImage.Text;
            ImageProfile.KernelArguments = txtKernelArgs.Text;
            var result = Call.ImageProfileApi.Put(ImageProfile.Id, ImageProfile);
            EndUserMessage = result.Success ? "Successfully Updated Image Profile" : result.ErrorMessage;
        }
    }
}