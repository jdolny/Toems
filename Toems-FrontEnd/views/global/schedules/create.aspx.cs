using System;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.global.schedules
{
    public partial class create : Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               PopulateForm();
            }
        }

        private void PopulateForm()
        {
            PopulateScheduleHour(ddlHour);
            PopulateMinute(ddlMinute);
        }
        
        protected void buttonAddSchedule_OnClick(object sender, EventArgs e)
        {
            var schedule = new EntitySchedule();
            schedule.Name = txtName.Text;
            schedule.Description = txtDescription.Text;
            schedule.Sunday = chkSunday.Checked;
            schedule.Monday = chkMonday.Checked;
            schedule.Tuesday = chkTuesday.Checked;
            schedule.Wednesday = chkWednesday.Checked;
            schedule.Thursday = chkThursday.Checked;
            schedule.Friday = chkFriday.Checked;
            schedule.Saturday = chkSaturday.Checked;
            schedule.Hour = Convert.ToInt16(ddlHour.SelectedValue);
            schedule.Minute = Convert.ToInt16(ddlMinute.SelectedValue);
            schedule.IsActive = true;

            var result = Call.ScheduleApi.Post(schedule);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Schedule";
                Response.Redirect("~/views/global/schedules/edit.aspx?scheduleId=" + result.Id);
            }

        }
    }
}