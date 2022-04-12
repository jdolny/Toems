using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.imageprep
{
    public partial class editsetupcomplete : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            txtName.Text = SetupCompleteFile.Name;
            txtDescription.Text = SetupCompleteFile.Description;
            scriptEditor.Value = SetupCompleteFile.Contents;
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {

            SetupCompleteFile.Name = txtName.Text;
            SetupCompleteFile.Description = txtDescription.Text;
            SetupCompleteFile.Contents = scriptEditor.Value;


            var result = Call.SetupCompleteFileApi.Put(SetupCompleteFile.Id, SetupCompleteFile);
            EndUserMessage = !result.Success ? result.ErrorMessage : "Successfully Updated Setup Complete File";
        }
    }
}