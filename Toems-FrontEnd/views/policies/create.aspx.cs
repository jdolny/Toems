using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.policies
{
    public partial class create : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
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
                PopulateRemoteAccess(ddlRemoteAccess);
                PopulateWuType(ddlWinUpdates);
                PopulatePolicyComCondition(ddlComCondition);
                PopulateConditions(ddlCondition);
                PopulateConditionFailedAction(ddlConditionFailedAction);

                FillFrequencies();
                FillSubFrequencies();
            }
        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            var policy = new EntityPolicy();
            policy.Name = txtName.Text;
            policy.Description = txtDescription.Text;
            policy.Trigger =
                (EnumPolicy.Trigger) Enum.Parse(typeof (EnumPolicy.Trigger), ddlTrigger.SelectedValue);
            policy.Frequency = (EnumPolicy.Frequency)Enum.Parse(typeof(EnumPolicy.Frequency), ddlFrequency.SelectedValue);
            if (policy.Frequency == EnumPolicy.Frequency.OncePerWeek)
                policy.SubFrequency = Convert.ToInt16((EnumCalendar.DayOfWeek)Enum.Parse(typeof(EnumCalendar.DayOfWeek), ddlWeekDay.SelectedValue));
            else if (policy.Frequency == EnumPolicy.Frequency.OncePerMonth)
                policy.SubFrequency = Convert.ToInt16(ddlMonthDay.SelectedValue);
            else if (policy.Frequency == EnumPolicy.Frequency.EveryXdays)
                policy.SubFrequency = Convert.ToInt16(txtDays.Text);
            else if (policy.Frequency == EnumPolicy.Frequency.EveryXhours)
                policy.SubFrequency = Convert.ToInt16(txtHours.Text);
            else
                policy.SubFrequency = 0;

            policy.WindowStartScheduleId = Convert.ToInt32(ddlScheduleStart.SelectedValue);
            policy.WindowEndScheduleId = Convert.ToInt32(ddlScheduleEnd.SelectedValue);
            policy.ConditionId = Convert.ToInt32(ddlCondition.SelectedValue);
            policy.ConditionFailedAction = (EnumCondition.FailedAction)Enum.Parse(typeof(EnumCondition.FailedAction), ddlConditionFailedAction.SelectedValue);
            //utc is not used here because time is not used only date
            policy.StartDate = string.IsNullOrEmpty(txtStartDate.Text) ? Convert.ToDateTime(DateTime.Now.ToShortDateString()) : Convert.ToDateTime(txtStartDate.Text);
            policy.CompletedAction = (EnumPolicy.CompletedAction)Enum.Parse(typeof(EnumPolicy.CompletedAction), ddlCompletedAction.SelectedValue);
            policy.RunInventory = (EnumPolicy.InventoryAction)Enum.Parse(typeof(EnumPolicy.InventoryAction), ddlInventory.SelectedValue);
            policy.RunLoginTracker = chkLoginTracker.Checked;
            policy.JoinDomain = chkJoinDomain.Checked;
            policy.DomainOU = txtDomainOU.Text;
            policy.ImagePrepCleanup = chkImagePrepCleanup.Checked;
            policy.RemoveInstallCache = chkDeleteCache.Checked;
            policy.RemoteAccess = (EnumPolicy.RemoteAccess)Enum.Parse(typeof(EnumPolicy.RemoteAccess), ddlRemoteAccess.SelectedValue);
            policy.MissedAction = (EnumPolicy.FrequencyMissedAction)Enum.Parse(typeof(EnumPolicy.FrequencyMissedAction), ddlMissed.SelectedValue);
            policy.ExecutionType = (EnumPolicy.ExecutionType)Enum.Parse(typeof(EnumPolicy.ExecutionType), ddlExecType.SelectedValue);
            policy.ErrorAction = (EnumPolicy.ErrorAction)Enum.Parse(typeof(EnumPolicy.ErrorAction), ddlErrorAction.SelectedValue);
            policy.LogLevel = (EnumPolicy.LogLevel)Enum.Parse(typeof(EnumPolicy.LogLevel), ddlLogLevel.SelectedValue);
            policy.WuType = (EnumPolicy.WuType)Enum.Parse(typeof(EnumPolicy.WuType), ddlWinUpdates.SelectedValue);
            policy.SkipServerResult = chkSkipResult.Checked;
            policy.RunApplicationMonitor = chkApplicationMonitor.Checked;
            policy.WingetUseMaxConnections = chkWingetDownloadConnections.Checked;
            policy.IsWingetUpdate = chkWingetUpdates.Checked;
            policy.AutoArchiveType = (EnumPolicy.AutoArchiveType)Enum.Parse(typeof(EnumPolicy.AutoArchiveType), ddlAutoArchive.SelectedValue);
            policy.PolicyComCondition = (EnumPolicy.PolicyComCondition)Enum.Parse(typeof(EnumPolicy.PolicyComCondition), ddlComCondition.SelectedValue);
            if (policy.AutoArchiveType == EnumPolicy.AutoArchiveType.AfterXdays)
                policy.AutoArchiveSub = txtAutoArchiveDays.Text;


            var result = Call.PolicyApi.Post(policy);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                if (policy.PolicyComCondition != EnumPolicy.PolicyComCondition.Any)
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
                        policyComServer.PolicyId = result.Id;

                        list.Add(policyComServer);
                    }

                    var z = Call.PolicyComServerApi.Post(list);
                    if (!z.Success)
                    {
                        EndUserMessage = z.ErrorMessage;
                        return;
                    }
                }

                EndUserMessage = "Successfully Created Policy";
                Response.Redirect("~/views/policies/general.aspx?policyId=" + result.Id);
            }
        }

        protected void ddlTrigger_OnSelectedIndexChanged(object sender, EventArgs e)
        {
           FillFrequencies();
        }

       

        private void FillFrequencies()
        {
            var trigger = (EnumPolicy.Trigger)Enum.Parse(typeof(EnumPolicy.Trigger), ddlTrigger.SelectedValue);
            FillAutoArchive();
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

        private void BindComServers()
        {
            var comCondition = (EnumPolicy.PolicyComCondition)Enum.Parse(typeof(EnumPolicy.PolicyComCondition),
                ddlComCondition.SelectedValue);
            if (comCondition == EnumPolicy.PolicyComCondition.Any)
                divComServers.Visible = false;
            else
            {

                divComServers.Visible = true;
                gvServers.DataSource = Call.ClientComServerApi.Get();
                gvServers.DataBind();
            }

        }
    }
}