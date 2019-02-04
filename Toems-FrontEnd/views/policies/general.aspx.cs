using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.policies
{
    public partial class general : BasePages.Policies
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
            PopulateCompletedAction(ddlCompletedAction);
            PopulateExecutionType(ddlExecType);
            PopulateDaysOfWeek(ddlWeekDay);
            PopulateDaysOfMonth(ddlMonthDay);
            PopulateScheduleDdl(ddlScheduleStart);
            PopulateScheduleDdl(ddlScheduleEnd);
            PopulateMissedAction(ddlMissed);
            PopulateLogLevels(ddlLogLevel);
            PopulateAutoArchive(ddlAutoArchive);
            PopulateErrorAction(ddlErrorAction);
            PopulateInventoryAction(ddlInventory);
            PopulateWuType(ddlWinUpdates);
            PopulatePolicyComCondition(ddlComCondition);
            FillFrequencies();
            FillSubFrequencies();

            txtName.Text = Policy.Name;
            txtDescription.Text = Policy.Description;
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
            ddlCompletedAction.SelectedValue = Policy.CompletedAction.ToString();
            ddlInventory.SelectedValue = Policy.RunInventory.ToString();
            chkLoginTracker.Checked = Policy.RunLoginTracker;
            chkApplicationMonitor.Checked = Policy.RunApplicationMonitor;
            chkDeleteCache.Checked = Policy.RemoveInstallCache;
            ddlExecType.SelectedValue = Policy.ExecutionType.ToString();
            ddlErrorAction.SelectedValue = Policy.ErrorAction.ToString();
            ddlWinUpdates.SelectedValue = Policy.WuType.ToString();
            ddlLogLevel.SelectedValue = Policy.LogLevel.ToString();
            ddlMissed.SelectedValue = Policy.MissedAction.ToString();
            chkSkipResult.Checked = Policy.SkipServerResult;
            ddlComCondition.SelectedValue = Policy.PolicyComCondition.ToString();
            ddlAutoArchive.SelectedValue = Policy.AutoArchiveType.ToString();
            if (Policy.AutoArchiveType == EnumPolicy.AutoArchiveType.AfterXdays)
                txtAutoArchiveDays.Text = Policy.AutoArchiveSub;

            ddlScheduleStart.SelectedValue = Policy.WindowStartScheduleId.ToString();
            ddlScheduleEnd.SelectedValue = Policy.WindowEndScheduleId.ToString();

            FillAutoArchive();
            FillSubFrequencies();
            BindComServers();
          

        }

        private void BindComServers()
        {
            var comCondition = (EnumPolicy.PolicyComCondition) Enum.Parse(typeof(EnumPolicy.PolicyComCondition),
                ddlComCondition.SelectedValue);
            if (comCondition == EnumPolicy.PolicyComCondition.Any)
                divComServers.Visible = false;
            else
            {
               
                divComServers.Visible = true;
                gvServers.DataSource = Call.ClientComServerApi.Get();
                gvServers.DataBind();

                var policyComServers = Call.PolicyApi.GetPolicyComServers(Policy.Id);
                var entityPolicyComServers = policyComServers as EntityPolicyComServer[] ?? policyComServers.ToArray();
                if (entityPolicyComServers.Any())
                {
                    foreach (GridViewRow row in gvServers.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        var dataKey = gvServers.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;

                        foreach (var comServer in entityPolicyComServers)
                        {
                            if (comServer.ComServerId == Convert.ToInt32(dataKey.Value))
                            {
                                cb.Checked = true;
                            }
                        }

                    }
                }
            }
          
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (Policy.Archived)
            {
                EndUserMessage = "Archived Policies Cannot Be Modified";
                return;
                
            }
            Policy.Name = txtName.Text;
            Policy.Description = txtDescription.Text;
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
            Policy.CompletedAction = (EnumPolicy.CompletedAction)Enum.Parse(typeof(EnumPolicy.CompletedAction), ddlCompletedAction.SelectedValue);
            Policy.RunInventory = (EnumPolicy.InventoryAction)Enum.Parse(typeof(EnumPolicy.InventoryAction), ddlInventory.SelectedValue);
            Policy.RunLoginTracker = chkLoginTracker.Checked;
            Policy.RemoveInstallCache = chkDeleteCache.Checked;
            Policy.WuType = (EnumPolicy.WuType)Enum.Parse(typeof(EnumPolicy.WuType), ddlWinUpdates.SelectedValue);
            Policy.ExecutionType = (EnumPolicy.ExecutionType)Enum.Parse(typeof(EnumPolicy.ExecutionType), ddlExecType.SelectedValue);
            Policy.ErrorAction = (EnumPolicy.ErrorAction)Enum.Parse(typeof(EnumPolicy.ErrorAction), ddlErrorAction.SelectedValue);
            Policy.MissedAction = (EnumPolicy.FrequencyMissedAction)Enum.Parse(typeof(EnumPolicy.FrequencyMissedAction), ddlMissed.SelectedValue);
            Policy.LogLevel = (EnumPolicy.LogLevel)Enum.Parse(typeof(EnumPolicy.LogLevel), ddlLogLevel.SelectedValue);
            Policy.PolicyComCondition= (EnumPolicy.PolicyComCondition)Enum.Parse(typeof(EnumPolicy.PolicyComCondition), ddlComCondition.SelectedValue);
            Policy.SkipServerResult = chkSkipResult.Checked;
            Policy.RunApplicationMonitor = chkApplicationMonitor.Checked;

            Policy.AutoArchiveType = (EnumPolicy.AutoArchiveType)Enum.Parse(typeof(EnumPolicy.AutoArchiveType), ddlAutoArchive.SelectedValue);
            if (Policy.AutoArchiveType == EnumPolicy.AutoArchiveType.AfterXdays)
                Policy.AutoArchiveSub = txtAutoArchiveDays.Text;

          

            if (Policy.PolicyComCondition == EnumPolicy.PolicyComCondition.Any)
            {
                var r= Call.PolicyComServerApi.Delete(Policy.Id);
                if (!r.Success)
                {
                    EndUserMessage = "Could Not Update Policy Com Servers.";
                    return;
                }
            }
            else
            {
                var list = new List<EntityPolicyComServer>();
                foreach (GridViewRow row in gvServers.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (cb == null || !cb.Checked) continue;
                    var dataKey = gvServers.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;
                    var policyComServer = new EntityPolicyComServer();
                    policyComServer.ComServerId = Convert.ToInt32(dataKey.Value);
                    policyComServer.PolicyId = Policy.Id;
                   
                    list.Add(policyComServer);
                }

                var z = Call.PolicyComServerApi.Post(list);
                if (!z.Success)
                {
                    EndUserMessage = z.ErrorMessage;
                    return;
                }
            }
            var result = Call.PolicyApi.Put(Policy.Id, Policy);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Policy {0}", Policy.Name) : result.ErrorMessage;
        }



    

      
       

        protected void ddlAutoArchive_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            FillAutoArchive();
        }

        private void FillAutoArchive()
        {
            var archiveType = (EnumPolicy.AutoArchiveType)Enum.Parse(typeof(EnumPolicy.AutoArchiveType), ddlAutoArchive.SelectedValue);
            divArchiveDays.Visible = archiveType == EnumPolicy.AutoArchiveType.AfterXdays;
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvServers);
        }

        protected void ddlComCondition_OnSelectedIndexChanged(object sender, EventArgs e)
        {
           BindComServers();
        }
    }
}