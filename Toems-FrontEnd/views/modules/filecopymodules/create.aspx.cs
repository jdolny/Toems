using System;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.modules.filecopymodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
         

        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            var module = new EntityFileCopyModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                Destination = txtDestination.Text,
                DecompressAfterCopy = chkUnzip.Checked,
            };


            var result = Call.FileCopyModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created File Copy Module";
                Response.Redirect("~/views/modules/filecopymodules/general.aspx?fileCopyModuleId=" + result.Id);
            }
        }
    }
}