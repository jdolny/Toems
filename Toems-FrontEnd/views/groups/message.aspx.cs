using System;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.groups
{
    public partial class message : BasePages.Groups
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.ComputerSendMessage);
        }

        protected void btnSend_OnClick(object sender, EventArgs e)
        {
            int timeout = 0;
            var parseResult = Int32.TryParse(txtTimeout.Text, out timeout);
            if (!parseResult)
            {
                EndUserMessage = "Timeout Is Not Valid";
                return;
            }
            if (timeout < 0)
            {
                EndUserMessage = "Timeout Is Not Valid";
                return;
            }

            var message = new DtoMessage();
            message.Timeout = timeout;
            message.Title = txtTitle.Text;
            message.Message = txtMessage.Text;
            Call.GroupApi.SendMessage(GroupEntity.Id, message);
            EndUserMessage = "Successfully Sent Message Request";
        }
    }
}