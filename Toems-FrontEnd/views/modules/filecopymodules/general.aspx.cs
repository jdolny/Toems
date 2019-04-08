using System;

namespace Toems_FrontEnd.views.modules.filecopymodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {

            txtDisplayName.Text = FileCopyModule.Name;
            txtGuid.Text = FileCopyModule.Guid;
            txtGuid.ReadOnly = true;
            txtDescription.Text = FileCopyModule.Description;
            txtDestination.Text = FileCopyModule.Destination;
            chkUnzip.Checked = FileCopyModule.DecompressAfterCopy;
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (FileCopyModule.Archived)
            {
                EndUserMessage = "Archived Modules Cannot Be Modified";
                return;
            }

            FileCopyModule.Name = txtDisplayName.Text;
            FileCopyModule.Description = txtDescription.Text;
            FileCopyModule.Destination = txtDestination.Text;
            FileCopyModule.DecompressAfterCopy = chkUnzip.Checked;

            var result = Call.FileCopyModuleApi.Put(FileCopyModule.Id, FileCopyModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", FileCopyModule.Name) : result.ErrorMessage;
        }



        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.FileCopyModuleApi.Delete(FileCopyModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", FileCopyModule.Name);
                Response.Redirect("~/views/modules/filecopymodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}