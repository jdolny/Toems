using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.modules.messagemodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {

            txtDisplayName.Text = MessageModule.Name;
            txtGuid.Text = MessageModule.Guid;
            txtGuid.ReadOnly = true;
            txtDescription.Text = MessageModule.Description;
            txtTitle.Text = MessageModule.Title;
            txtMessage.Text = MessageModule.Message;
            txtTimeout.Text = MessageModule.Timeout.ToString();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (MessageModule.Archived)
            {
                EndUserMessage = "Archived Modules Cannot Be Modified";
                return;
            }

            MessageModule.Name = txtDisplayName.Text;
            MessageModule.Description = txtDescription.Text;
            MessageModule.Title = txtTitle.Text;
            MessageModule.Message = txtMessage.Text;
            MessageModule.Timeout = Convert.ToInt32(txtTimeout.Text);

            var result = Call.MessageModuleApi.Put(MessageModule.Id, MessageModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", MessageModule.Name) : result.ErrorMessage;
        }



        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.MessageModuleApi.Delete(MessageModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", MessageModule.Name);
                Response.Redirect("~/views/modules/messagemodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}