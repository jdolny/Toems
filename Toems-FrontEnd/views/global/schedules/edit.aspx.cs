using System;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.global.schedules
{
    public partial class edit : Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdateSchedule_OnClick(object sender, EventArgs e)
        {
            var schedule = Call.ScheduleApi.Get(Convert.ToInt32(Request.QueryString["scheduleId"]));
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
            schedule.IsActive = chkActive.Checked;

            var result = Call.ScheduleApi.Put(schedule.Id, schedule);
            EndUserMessage = result.Success ? "Successfully Updated Schedule" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            PopulateScheduleHour(ddlHour);
            PopulateMinute(ddlMinute);

            var schedule = Call.ScheduleApi.Get(Convert.ToInt32(Request.QueryString["scheduleId"]));
            txtName.Text = schedule.Name;
            txtDescription.Text = schedule.Description;
            chkSunday.Checked = schedule.Sunday;
            chkMonday.Checked = schedule.Monday;
            chkTuesday.Checked = schedule.Tuesday;
            chkWednesday.Checked = schedule.Wednesday;
            chkThursday.Checked = schedule.Thursday;
            chkFriday.Checked = schedule.Friday;
            chkSaturday.Checked = schedule.Saturday;
            ddlHour.SelectedValue = schedule.Hour.ToString();
            ddlMinute.SelectedValue = schedule.Minute.ToString();
            chkActive.Checked = schedule.IsActive;
        }

      
    }
}