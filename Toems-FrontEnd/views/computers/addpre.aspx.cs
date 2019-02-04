using System;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.computers
{
    public partial class addpre : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnAddPre_OnClick(object sender, EventArgs e)
        {
            var seperator = new string[] { "\r\n" };
            foreach (var name in txtPre.Text.Split(seperator, StringSplitOptions.RemoveEmptyEntries))
            {
                var computer = new EntityComputer();
                computer.Name = name;
                computer.ProvisionStatus = EnumProvisionStatus.Status.PreProvisioned;
                var result = Call.ComputerApi.Post(computer);
                if (!result.Success)
                {
                    EndUserMessage = "Could Not PreProvision " + name + " " + result.ErrorMessage;
                    break;
                }
                else
                {
                    EndUserMessage = "Success";
                }

            }
        
        }
    }
}