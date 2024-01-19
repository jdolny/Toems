using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.wingetmodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {

            var module = new EntityWingetModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                Timeout = 0,
                Arguments = string.Empty,
                Override = string.Empty,
                KeepUpdated = false,
                InstallLatest = false,
                InstallType = EnumWingetInstallType.WingetInstallType.Install,
                RedirectStdOut = false,
                RedirectStdError = false,
            };


            var result = Call.WingetModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Winget Module";
                Response.Redirect("~/views/modules/wingetmodules/general.aspx?wingetModuleId=" + result.Id);
            }
        }
    }
}