using System;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.computers
{
    public partial class message : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.ComputerSendMessage);
        }

        protected void btnSend_OnClick(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(ComputerEntity.PushUrl))
            {
                EndUserMessage = "Messages Can Only Be Sent To Computers That Have Checked In.";
                return;
            }
            int timeout = 0;
            var parseResult = Int32.TryParse(txtTimeout.Text, out timeout);
            if (!parseResult)
            {
                EndUserMessage = "Timeout Is Not Valid";
                return;
            }
            if(timeout < 0)
            {
                EndUserMessage = "Timeout Is Not Valid";
                return;
            }

            var message = new DtoMessage();
            message.Timeout = timeout;
            message.Title = txtTitle.Text;
            message.Message = txtMessage.Text;
            var result = Call.ComputerApi.SendMessage(ComputerEntity.Id, message);
            EndUserMessage = result ? "Successfully Sent Message." : "Could Not Send Message.";
        }
    }
}