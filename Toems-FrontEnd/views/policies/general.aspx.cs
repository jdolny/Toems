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
     
        protected void PopulateForm()
        {
            PopulateCompletedAction(ddlCompletedAction);
            PopulateExecutionType(ddlExecType);
            PopulateLogLevels(ddlLogLevel);
            PopulateAutoArchive(ddlAutoArchive);
            PopulateErrorAction(ddlErrorAction);


            txtName.Text = Policy.Name;
            txtDescription.Text = Policy.Description;


         
            ddlCompletedAction.SelectedValue = Policy.CompletedAction.ToString();
            chkDeleteCache.Checked = Policy.RemoveInstallCache;
            ddlExecType.SelectedValue = Policy.ExecutionType.ToString();
            ddlErrorAction.SelectedValue = Policy.ErrorAction.ToString();
            ddlLogLevel.SelectedValue = Policy.LogLevel.ToString();
            chkSkipResult.Checked = Policy.SkipServerResult;
            ddlAutoArchive.SelectedValue = Policy.AutoArchiveType.ToString();
            if (Policy.AutoArchiveType == EnumPolicy.AutoArchiveType.AfterXdays)
                txtAutoArchiveDays.Text = Policy.AutoArchiveSub;


            FillAutoArchive();
        
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
         
            Policy.CompletedAction = (EnumPolicy.CompletedAction)Enum.Parse(typeof(EnumPolicy.CompletedAction), ddlCompletedAction.SelectedValue);
            Policy.RemoveInstallCache = chkDeleteCache.Checked;
            Policy.ExecutionType = (EnumPolicy.ExecutionType)Enum.Parse(typeof(EnumPolicy.ExecutionType), ddlExecType.SelectedValue);
            Policy.ErrorAction = (EnumPolicy.ErrorAction)Enum.Parse(typeof(EnumPolicy.ErrorAction), ddlErrorAction.SelectedValue);
            Policy.LogLevel = (EnumPolicy.LogLevel)Enum.Parse(typeof(EnumPolicy.LogLevel), ddlLogLevel.SelectedValue);
            Policy.SkipServerResult = chkSkipResult.Checked;
            Policy.AutoArchiveType = (EnumPolicy.AutoArchiveType)Enum.Parse(typeof(EnumPolicy.AutoArchiveType), ddlAutoArchive.SelectedValue);
            if (Policy.AutoArchiveType == EnumPolicy.AutoArchiveType.AfterXdays)
                Policy.AutoArchiveSub = txtAutoArchiveDays.Text;

         
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

    }
}