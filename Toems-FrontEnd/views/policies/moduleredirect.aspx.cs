using System;

namespace Toems_FrontEnd.views.policies
{
    public partial class moduleredirect : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var type = Request.QueryString["type"];
            var id = Request.QueryString["id"];
            if(type == "Printer")
                 Response.Redirect("~/views/modules/printermodules/general.aspx?printerModuleId=" + id);
            if(type == "Software")
                 Response.Redirect("~/views/modules/softwaremodules/general.aspx?softwareModuleId=" + id);
             if(type == "Script")
                 Response.Redirect("~/views/modules/scriptmodules/general.aspx?scriptModuleId=" + id);
             if(type == "Command")
                 Response.Redirect("~/views/modules/commandmodules/general.aspx?commandModuleId=" + id);
             if(type == "FileCopy")
                 Response.Redirect("~/views/modules/filecopymodules/general.aspx?fileCopyModuleId=" + id);
            if (type == "Wupdate")
                Response.Redirect("~/views/modules/wumodules/general.aspx?wuModuleId=" + id);
            if (type == "WinPE")
                Response.Redirect("~/views/modules/winpemodules/general.aspx?winPeModuleId=" + id);
            if (type == "Winget")
                Response.Redirect("~/views/modules/wingetmodules/general.aspx?wingetModuleId=" + id);

        }
    }
}