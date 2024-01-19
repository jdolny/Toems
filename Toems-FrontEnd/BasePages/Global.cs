using System;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.BasePages
{
    public class Global : PageBaseMaster
    {
        public EntitySchedule Schedule { get; set; }
        public EntityProcessInventory Process { get; set; }
        public EntitySoftwareInventory Software { get; set; }
        public EntityCategory Category { get; set; }
        public EntityCustomAttribute CustomAttribute { get; set; }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Schedule = !string.IsNullOrEmpty(Request["scheduleId"])
               ? Call.ScheduleApi.Get(Convert.ToInt32(Request.QueryString["scheduleId"]))
               : null;
            Process = !string.IsNullOrEmpty(Request["processId"])
             ? Call.ProcessInventoryApi.Get(Convert.ToInt32(Request.QueryString["processId"]))
             : null;
            Software = !string.IsNullOrEmpty(Request["softwareId"])
             ? Call.SoftwareInventoryApi.Get(Convert.ToInt32(Request.QueryString["softwareId"]))
             : null;
            Category = !string.IsNullOrEmpty(Request["categoryId"])
                ? Call.CategoryApi.Get(Convert.ToInt32(Request.QueryString["categoryId"]))
                : null;
            CustomAttribute = !string.IsNullOrEmpty(Request["attributeId"])
             ? Call.CustomAttributeApi.Get(Convert.ToInt32(Request.QueryString["attributeId"]))
             : null;

        }

        protected void PopulateScheduleHour(DropDownList ddl)
        {
            ddl.Items.Insert(0, new ListItem("0", "0"));
            ddl.Items.Insert(1, new ListItem("1", "1"));
            ddl.Items.Insert(2, new ListItem("2", "2"));
            ddl.Items.Insert(3, new ListItem("3", "3"));
            ddl.Items.Insert(4, new ListItem("4", "4"));
            ddl.Items.Insert(5, new ListItem("5", "5"));
            ddl.Items.Insert(6, new ListItem("6", "6"));
            ddl.Items.Insert(7, new ListItem("7", "7"));
            ddl.Items.Insert(8, new ListItem("8", "8"));
            ddl.Items.Insert(9, new ListItem("9", "9"));
            ddl.Items.Insert(10, new ListItem("10", "10"));
            ddl.Items.Insert(11, new ListItem("11", "11"));
            ddl.Items.Insert(12, new ListItem("12", "12"));
            ddl.Items.Insert(13, new ListItem("13", "13"));
            ddl.Items.Insert(14, new ListItem("14", "14"));
            ddl.Items.Insert(15, new ListItem("15", "15"));
            ddl.Items.Insert(16, new ListItem("16", "16"));
            ddl.Items.Insert(17, new ListItem("17", "17"));
            ddl.Items.Insert(18, new ListItem("18", "18"));
            ddl.Items.Insert(19, new ListItem("19", "19"));
            ddl.Items.Insert(20, new ListItem("20", "20"));
            ddl.Items.Insert(21, new ListItem("21", "21"));
            ddl.Items.Insert(22, new ListItem("22", "22"));
            ddl.Items.Insert(23, new ListItem("23", "23"));

        }

        protected void PopulateMinute(DropDownList ddl)
        {
            ddl.Items.Insert(0, new ListItem("0","0"));
            ddl.Items.Insert(1, new ListItem("15", "15"));
            ddl.Items.Insert(2, new ListItem("30", "30"));
            ddl.Items.Insert(3, new ListItem("45", "45"));
        }

        protected void PopulateTextMode(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumCustomAttribute.TextMode));
            ddl.DataBind();
        }

        protected void PopulateCustomAttributeUsageType(DropDownList ddlUsageType)
        {
            ddlUsageType.DataSource = Call.CustomAssetTypeApi.Get().Select(d => new { d.Id, d.Name });
            ddlUsageType.DataValueField = "Id";
            ddlUsageType.DataTextField = "Name";
            ddlUsageType.DataBind();
        }



    }
}