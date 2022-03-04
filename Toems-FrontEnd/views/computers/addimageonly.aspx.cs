using System;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.computers
{
    public partial class addimageonly : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnCreate_OnClick(object sender, EventArgs e)
        {
            var seperator = new string[] { "\r\n" };
            foreach (var obj in txtImageOnly.Text.Split(seperator, StringSplitOptions.RemoveEmptyEntries))
            {

                try
                {
                    var comp = obj.Split(',');

                    var computer = new EntityComputer();
                    computer.Name = comp[0];
                    computer.ProvisionStatus = EnumProvisionStatus.Status.ImageOnly;
                    computer.ImagingMac = comp[1];
                    computer.ImagingClientId = comp[1];
                    var result = Call.ComputerApi.Post(computer);
                    if (!result.Success)
                    {
                        EndUserMessage = "Could Not create " + obj + " " + result.ErrorMessage;
                        break;
                    }
                    else
                    {
                        EndUserMessage = "Complete";
                    }
                }
                catch
                {
                    EndUserMessage = "Could Not create " + obj;
                    break;
                }
               

            }
            
        
        }
    }
}