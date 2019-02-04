using System;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.BasePages
{
    public class Computers : PageBaseMaster
    {
        public EntityComputer ComputerEntity { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RequiresAuthorization(AuthorizationStrings.ComputerRead);
            ComputerEntity = !string.IsNullOrEmpty(Request.QueryString["computerId"])
               ? Call.ComputerApi.Get(Convert.ToInt32(Request.QueryString["computerId"]))
               : null;
        }

        protected void PopulateTriggers(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.Trigger));
            ddl.DataBind();
        }
    }
}