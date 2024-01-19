using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.policies
{
    public partial class triggers : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateForm();
            }
        }

        protected void ddlTrigger_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            FillFrequencies();
        }

        private void FillFrequencies()
        {
            var trigger = (EnumPolicy.Trigger)Enum.Parse(typeof(EnumPolicy.Trigger), ddlTrigger.SelectedValue);
            PopulateFrequency(ddlFrequency, trigger);
        }

        protected void ddlFrequency_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            FillSubFrequencies();
        }

        private void FillSubFrequencies()
        {
            var frequency = (EnumPolicy.Frequency)Enum.Parse(typeof(EnumPolicy.Frequency), ddlFrequency.SelectedValue);
            if (frequency == EnumPolicy.Frequency.OncePerWeek)
            {
                divMonthly.Visible = false;
                divXHours.Visible = false;
                divXDays.Visible = false;
                divWeekly.Visible = true;
                divMissed.Visible = true;
            }
            else if (frequency == EnumPolicy.Frequency.OncePerMonth)
            {
                divMonthly.Visible = true;
                divXHours.Visible = false;
                divXDays.Visible = false;
                divWeekly.Visible = false;
                divMissed.Visible = true;
            }
            else if (frequency == EnumPolicy.Frequency.EveryXdays)
            {
                divMonthly.Visible = false;
                divXHours.Visible = false;
                divXDays.Visible = true;
                divWeekly.Visible = false;
                divMissed.Visible = false;
            }
            else if (frequency == EnumPolicy.Frequency.EveryXhours)
            {
                divMonthly.Visible = false;
                divXHours.Visible = true;
                divXDays.Visible = false;
                divWeekly.Visible = false;
                divMissed.Visible = false;
            }
            else
            {
                divMonthly.Visible = false;
                divWeekly.Visible = false;
                divMissed.Visible = false;
                divXHours.Visible = false;
                divXDays.Visible = false;
            }
        }

        protected void PopulateForm()
        {
            PopulateTriggers(ddlTrigger);       
            PopulateDaysOfWeek(ddlWeekDay);
            PopulateDaysOfMonth(ddlMonthDay);
            PopulateScheduleDdl(ddlScheduleStart);
            PopulateScheduleDdl(ddlScheduleEnd);
            PopulateMissedAction(ddlMissed);
            FillFrequencies();
            FillSubFrequencies();


            ddlTrigger.SelectedValue = Policy.Trigger.ToString();

            ddlFrequency.SelectedValue = Policy.Frequency.ToString();
            if (Policy.Frequency == EnumPolicy.Frequency.OncePerWeek)
                ddlWeekDay.SelectedIndex = Policy.SubFrequency;
            else if (Policy.Frequency == EnumPolicy.Frequency.OncePerMonth)
                ddlMonthDay.SelectedIndex = Policy.SubFrequency;
            else if (Policy.Frequency == EnumPolicy.Frequency.EveryXdays)
                txtDays.Text = Policy.SubFrequency.ToString();
            else if (Policy.Frequency == EnumPolicy.Frequency.EveryXhours)
                txtHours.Text = Policy.SubFrequency.ToString();

            txtStartDate.Text = Policy.StartDate.ToShortDateString();
          
            ddlMissed.SelectedValue = Policy.MissedAction.ToString();
         

            ddlScheduleStart.SelectedValue = Policy.WindowStartScheduleId.ToString();
            ddlScheduleEnd.SelectedValue = Policy.WindowEndScheduleId.ToString();

            FillSubFrequencies();

        }



        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (Policy.Archived)
            {
                EndUserMessage = "Archived Policies Cannot Be Modified";
                return;

            }
            Policy.Trigger =
                (EnumPolicy.Trigger)Enum.Parse(typeof(EnumPolicy.Trigger), ddlTrigger.SelectedValue);
            Policy.Frequency = (EnumPolicy.Frequency)Enum.Parse(typeof(EnumPolicy.Frequency), ddlFrequency.SelectedValue);
            if (Policy.Frequency == EnumPolicy.Frequency.OncePerWeek)
                Policy.SubFrequency = Convert.ToInt16((EnumCalendar.DayOfWeek)Enum.Parse(typeof(EnumCalendar.DayOfWeek), ddlWeekDay.SelectedValue));
            else if (Policy.Frequency == EnumPolicy.Frequency.OncePerMonth)
                Policy.SubFrequency = Convert.ToInt16(ddlMonthDay.SelectedValue);
            else if (Policy.Frequency == EnumPolicy.Frequency.EveryXdays)
                Policy.SubFrequency = Convert.ToInt16(txtDays.Text);
            else if (Policy.Frequency == EnumPolicy.Frequency.EveryXhours)
                Policy.SubFrequency = Convert.ToInt16(txtHours.Text);
            else
                Policy.SubFrequency = 0;

            Policy.WindowStartScheduleId = Convert.ToInt32(ddlScheduleStart.SelectedValue);
            Policy.WindowEndScheduleId = Convert.ToInt32(ddlScheduleEnd.SelectedValue);
            Policy.StartDate = string.IsNullOrEmpty(txtStartDate.Text) ? Convert.ToDateTime(DateTime.UtcNow.ToShortDateString()) : Convert.ToDateTime(txtStartDate.Text).ToUniversalTime();
            Policy.MissedAction = (EnumPolicy.FrequencyMissedAction)Enum.Parse(typeof(EnumPolicy.FrequencyMissedAction), ddlMissed.SelectedValue);
         
            var result = Call.PolicyApi.Put(Policy.Id, Policy);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Policy {0}", Policy.Name) : result.ErrorMessage;
        }
    }
}