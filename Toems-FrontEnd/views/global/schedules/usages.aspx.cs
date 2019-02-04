using System;

namespace Toems_FrontEnd.views.global.schedules
{
    public partial class usages : BasePages.Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           BindGrid();
        }



        private void BindGrid()
        {
            if (ddlType.Text.Equals("Groups"))
            {
                gvComputers.Visible = false;
                gvGroups.Visible = true;
                gvPolicies.Visible = false;
                gvGroups.DataSource =
                    Call.ScheduleApi.GetScheduleGroups(Convert.ToInt32(Request.QueryString["scheduleId"]),
                        ddlObjectType.Text);
                gvGroups.DataBind();
            }
            else if (ddlType.Text.Equals("Computers"))
            {
                gvComputers.Visible = true;
                gvGroups.Visible = false;
                gvPolicies.Visible = false;
                gvComputers.DataSource =
                    Call.ScheduleApi.GetScheduleComputers(Convert.ToInt32(Request.QueryString["scheduleId"]),
                        ddlObjectType.Text);
                gvComputers.DataBind();
            }
            else if (ddlType.Text.Equals("Policies"))
            {
                gvComputers.Visible = false;
                gvGroups.Visible = false;
                gvPolicies.Visible = true;
                gvPolicies.DataSource =
                    Call.ScheduleApi.GetSchedulePolicies(Convert.ToInt32(Request.QueryString["scheduleId"]),
                        ddlObjectType.Text);
                gvPolicies.DataBind();
            }
        }

        protected void ddlObjectType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
           BindGrid();
        }
    }
}