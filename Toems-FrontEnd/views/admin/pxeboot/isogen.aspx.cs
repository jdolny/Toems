using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.admin.pxeboot
{
    public partial class isogen : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.PxeISOGen);
            if (!IsPostBack)
            {
                PopulateClusterGroupsDdl(ddlCluster);
                ddlKernel.DataSource = Call.FilesystemApi.GetKernels();
                ddlBootImage.DataSource = Call.FilesystemApi.GetBootImages();
                ddlKernel.DataBind();
                ddlBootImage.DataBind();
                ddlKernel.SelectedValue = SettingStrings.DefaultKernel64;
                ddlBootImage.SelectedValue = SettingStrings.DefaultInit;
                chkSecureBoot.Checked = true;
            }
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            var isoGenOptions = new DtoIsoGenOptions();
            isoGenOptions.bootImage = ddlBootImage.Text;
            isoGenOptions.kernel = ddlKernel.Text;
            isoGenOptions.arguments = txtKernelArgs.Text;
            isoGenOptions.clusterId = Convert.ToInt32(ddlCluster.SelectedValue);
            isoGenOptions.useSecureBoot = chkSecureBoot.Checked;
            var clientboot = Call.SettingApi.GenerateIso(isoGenOptions);

            Response.Clear();
            var ms = new MemoryStream(clientboot);
            Response.ContentType = "application/iso";
            Response.AddHeader("content-disposition", "attachment;filename=clientboot.iso");


            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }
    }
}