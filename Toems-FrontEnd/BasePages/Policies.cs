using System;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.BasePages
{
    public class Policies : PageBaseMaster
    {
        public EntityPolicy Policy { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RequiresAuthorization(AuthorizationStrings.PolicyRead);
            Policy = !string.IsNullOrEmpty(Request.QueryString["policyId"])
               ? Call.PolicyApi.Get(Convert.ToInt32(Request.QueryString["policyId"]))
               : null;
        }

        protected void PopulateErrorAction(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.ErrorAction));
            ddl.DataBind();

        }

        protected void PopulateInventoryAction(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.InventoryAction));
            ddl.DataBind();

        }

        protected void PopulateAutoArchive(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.AutoArchiveType));
            ddl.DataBind();
        }

        protected void PopulateTriggers(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.Trigger));
            ddl.DataBind();
        }

        protected void PopulateWuType(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.WuType));
            ddl.DataBind();
        }

        protected void PopulatePolicyComCondition(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.PolicyComCondition));
            ddl.DataBind();
        }

        protected void PopulateMissedAction(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof (EnumPolicy.FrequencyMissedAction));
            ddl.DataBind();
        }

        protected void PopulateLogLevels(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof (EnumPolicy.LogLevel));
            ddl.DataBind();
        }

        protected void PopulateFrequency(DropDownList ddl, EnumPolicy.Trigger trigger)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.Frequency));
            ddl.DataBind();

            if (trigger == EnumPolicy.Trigger.Login)
            {
                ddl.Items.Remove("OncePerComputer");
                ddl.Items.Remove("OncePerDay");
                ddl.Items.Remove("OncePerWeek");
                ddl.Items.Remove("OncePerMonth");
                ddl.Items.Remove("EveryXdays");
                ddl.Items.Remove("EveryXhours");
            }
            else
            {
                ddl.Items.Remove("OncePerUserPerComputer");
            }
        }

        protected void PopulateCompletedAction(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.CompletedAction));
            ddl.DataBind();
        }

        protected void PopulateExecutionType(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumPolicy.ExecutionType));
            ddl.DataBind();
        }

        protected void PopulateDaysOfWeek(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumCalendar.DayOfWeek));
            ddl.DataBind();
        }

        protected void PopulateDaysOfMonth(DropDownList ddl)
        {
            for(var d =1; d <=31;d++)
            ddl.Items.Add(new ListItem(d.ToString(), d.ToString()));
        }
    }
}