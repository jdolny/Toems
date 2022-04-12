using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.imageprep
{
    public partial class createsysprep : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            var sysprep = new EntitySysprepAnswerfile()
            {
                Name = txtName.Text,
                Description = txtDescription.Text,
                Contents = scriptEditor.Value
            };

            var result = Call.SysprepAnswerFileApi.Post(sysprep);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Added Sysprep Answer File";
                Response.Redirect("~/views/admin/imageprep/editsysprep.aspx?&sysprepId=" + result.Id + "&level=2");
            }
        }
    }
}