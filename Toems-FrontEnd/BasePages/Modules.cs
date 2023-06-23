using System;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.BasePages
{
    public class Modules : PageBaseMaster
    {
        public EntityPrinterModule PrinterModule { get; set; }
        public EntitySoftwareModule SoftwareModule { get; set; }
        public EntityCommandModule CommandModule { get; set; }
        public EntityFileCopyModule FileCopyModule { get; set; }
        public EntityScriptModule ScriptModule { get; set; }
        public EntityWuModule WuModule { get; set; }
        public EntityMessageModule MessageModule { get; set; }
        public EntitySysprepModule SysprepModule { get; set; }
        public EntityWinPeModule WinPeModule { get; set; }
        public EntityWingetModule WingetModule { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RequiresAuthorization(AuthorizationStrings.ModuleRead);
            PrinterModule = !string.IsNullOrEmpty(Request["printerModuleId"])
               ? Call.PrinterModuleApi.Get(Convert.ToInt32(Request.QueryString["printerModuleId"]))
               : null;

            SoftwareModule = !string.IsNullOrEmpty(Request["softwareModuleId"])
               ? Call.SoftwareModuleApi.Get(Convert.ToInt32(Request.QueryString["softwareModuleId"]))
               : null;

            CommandModule = !string.IsNullOrEmpty(Request["commandModuleId"])
              ? Call.CommandModuleApi.Get(Convert.ToInt32(Request.QueryString["commandModuleId"]))
              : null;

            FileCopyModule = !string.IsNullOrEmpty(Request["fileCopyModuleId"])
              ? Call.FileCopyModuleApi.Get(Convert.ToInt32(Request.QueryString["fileCopyModuleId"]))
              : null;

            ScriptModule = !string.IsNullOrEmpty(Request["scriptModuleId"])
              ? Call.ScriptModuleApi.Get(Convert.ToInt32(Request.QueryString["scriptModuleId"]))
              : null;

            WuModule = !string.IsNullOrEmpty(Request["wuModuleId"])
                ? Call.WuModuleApi.Get(Convert.ToInt32(Request.QueryString["wuModuleId"]))
                : null;
            MessageModule = !string.IsNullOrEmpty(Request["messageModuleId"])
               ? Call.MessageModuleApi.Get(Convert.ToInt32(Request.QueryString["messageModuleId"]))
               : null;
            SysprepModule = !string.IsNullOrEmpty(Request["sysprepModuleId"])
              ? Call.SysprepModuleApi.Get(Convert.ToInt32(Request.QueryString["sysprepModuleId"]))
              : null;
            WinPeModule = !string.IsNullOrEmpty(Request["winPeModuleId"])
             ? Call.WinPeModuleApi.Get(Convert.ToInt32(Request.QueryString["winPeModuleId"]))
             : null;
            WingetModule = !string.IsNullOrEmpty(Request["wingetModuleId"])
            ? Call.WingetModuleApi.Get(Convert.ToInt32(Request.QueryString["wingetModuleId"]))
            : null;


        }

     
    }
}