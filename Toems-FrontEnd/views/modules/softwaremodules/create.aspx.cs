using System;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.softwaremodules
{
    public partial class create : BasePages.Modules
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlType.DataSource = Enum.GetNames(typeof(EnumSoftwareModule.MsiInstallType));
                ddlType.DataBind();
                PopulateImpersonationDdl(ddlRunAs);
            }

        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            int timeout = 0;
            var parseResult = Int32.TryParse(txtTimeout.Text, out timeout);
            if (!parseResult)
            {
                EndUserMessage = "Timeout Is Not Valid";
                return;
            }

            var module = new EntitySoftwareModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                InstallType = (EnumSoftwareModule.MsiInstallType)Enum.Parse(typeof(EnumSoftwareModule.MsiInstallType), ddlType.SelectedValue),
                Timeout = Convert.ToInt32(txtTimeout.Text),
                RedirectStdOut = chkStdOut.Checked,
                RedirectStdError = chkStdError.Checked,
                SuccessCodes = txtSuccessCodes.Text,
                ImpersonationId = Convert.ToInt32(ddlRunAs.SelectedValue)
                


            };


            var result = Call.SoftwareModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Software Module";
                Response.Redirect("~/views/modules/softwaremodules/general.aspx?softwareModuleId=" + result.Id);
            }
        }

      
    }
}