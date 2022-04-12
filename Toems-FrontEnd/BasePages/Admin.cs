using System;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.BasePages
{
    public class Admin : PageBaseMaster
    {
       
        public EntityImpersonationAccount ImpersonationAccount { get; set; }
        public EntityClientComServer ComServer { get; set; }
        public EntityComServerCluster ComServerCluster { get; set; }
        public EntityWolRelay WolRelay { get; set; }
        public EntityCustomBootMenu BootEntry { get; set; }

        public EntitySysprepAnswerfile SysprepFile { get; set; }

        public EntitySetupCompleteFile SetupCompleteFile { get; set; }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
           //auth not checked here, to allow pxe updates / gen by non admin
            ImpersonationAccount = !string.IsNullOrEmpty(Request["impersonationId"])
                ? Call.ImpersonationAccountApi.Get(Convert.ToInt32(Request.QueryString["impersonationId"]))
                : null;

            ComServer = !string.IsNullOrEmpty(Request["serverId"])
                ? Call.ClientComServerApi.Get(Convert.ToInt32(Request.QueryString["serverId"]))
                : null;

            ComServerCluster = !string.IsNullOrEmpty(Request["clusterId"])
                ? Call.ComServerClusterApi.Get(Convert.ToInt32(Request.QueryString["clusterId"]))
                : null;

            WolRelay = !string.IsNullOrEmpty(Request["relayId"])
                ? Call.WolRelayApi.Get(Convert.ToInt32(Request.QueryString["relayId"]))
                : null;
            BootEntry = !string.IsNullOrEmpty(Request["entryId"])
              ? Call.CustomBootMenuApi.Get(Convert.ToInt32(Request.QueryString["entryId"]))
              : null;
            SysprepFile = !string.IsNullOrEmpty(Request["sysprepId"])
             ? Call.SysprepAnswerFileApi.Get(Convert.ToInt32(Request.QueryString["sysprepId"]))
             : null;
            SetupCompleteFile = !string.IsNullOrEmpty(Request["setupCompleteId"])
             ? Call.SetupCompleteFileApi.Get(Convert.ToInt32(Request.QueryString["setupCompleteId"]))
             : null;

        }

        protected void PopulateStartupDelay(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.StartupDelayType));
            ddl.DataBind();
        }
    }
}