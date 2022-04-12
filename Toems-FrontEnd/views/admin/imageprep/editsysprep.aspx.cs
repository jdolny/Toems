using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.imageprep
{
    public partial class editsysprep : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            txtName.Text = SysprepFile.Name;
            txtDescription.Text = SysprepFile.Description;
            scriptEditor.Value = SysprepFile.Contents;
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {

            SysprepFile.Name = txtName.Text;
            SysprepFile.Description = txtDescription.Text;
            SysprepFile.Contents = scriptEditor.Value;
          

            var result = Call.SysprepAnswerFileApi.Put(SysprepFile.Id, SysprepFile);
            EndUserMessage = !result.Success ? result.ErrorMessage : "Successfully Updated Sysprep Answer File";
        }
    }
}