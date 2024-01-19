using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.policies
{
    public partial class specialties : BasePages.Policies
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
            PopulateInventoryAction(ddlInventory);
            PopulateRemoteAccess(ddlRemoteAccess);
            PopulateWuType(ddlWinUpdates);


            ddlInventory.SelectedValue = Policy.RunInventory.ToString();
            ddlRemoteAccess.SelectedValue = Policy.RemoteAccess.ToString();
            chkLoginTracker.Checked = Policy.RunLoginTracker;
            chkApplicationMonitor.Checked = Policy.RunApplicationMonitor;
            chkJoinDomain.Checked = Policy.JoinDomain;
            txtDomainOU.Text = Policy.DomainOU;
            chkImagePrepCleanup.Checked = Policy.ImagePrepCleanup;
            chkWingetUpdates.Checked = Policy.IsWingetUpdate;
            chkWingetDownloadConnections.Checked = Policy.WingetUseMaxConnections;
            ddlWinUpdates.SelectedValue = Policy.WuType.ToString();
        }

       

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (Policy.Archived)
            {
                EndUserMessage = "Archived Policies Cannot Be Modified";
                return;

            }


     
            Policy.RunInventory = (EnumPolicy.InventoryAction)Enum.Parse(typeof(EnumPolicy.InventoryAction), ddlInventory.SelectedValue);
            Policy.RemoteAccess = (EnumPolicy.RemoteAccess)Enum.Parse(typeof(EnumPolicy.RemoteAccess), ddlRemoteAccess.SelectedValue);
            Policy.RunLoginTracker = chkLoginTracker.Checked;
            Policy.JoinDomain = chkJoinDomain.Checked;
            Policy.DomainOU = txtDomainOU.Text;
            Policy.ImagePrepCleanup = chkImagePrepCleanup.Checked;
            Policy.WuType = (EnumPolicy.WuType)Enum.Parse(typeof(EnumPolicy.WuType), ddlWinUpdates.SelectedValue);      
            Policy.RunApplicationMonitor = chkApplicationMonitor.Checked;
            Policy.WingetUseMaxConnections = chkWingetDownloadConnections.Checked;
            Policy.IsWingetUpdate = chkWingetUpdates.Checked;


            var result = Call.PolicyApi.Put(Policy.Id, Policy);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Policy {0}", Policy.Name) : result.ErrorMessage;
        }
    }
}