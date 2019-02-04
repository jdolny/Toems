using System;

namespace Toems_FrontEnd.views.groups
{
    public partial class general : BasePages.Groups
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
            PopulateClusterGroupsDdl(ddlCluster);
            PopulateScheduleDdl(ddlWakeup);
            PopulateScheduleDdl(ddlShutdown);
            txtName.Text = GroupEntity.Name;
            txtDescription.Text = GroupEntity.Description;
            ddlGroupType.Text = GroupEntity.Type;
            ddlGroupType.Enabled = false;
            ddlCluster.SelectedValue = GroupEntity.ClusterId.ToString();
            ddlWakeup.SelectedValue = GroupEntity.WakeupScheduleId.ToString();
            ddlShutdown.SelectedValue = GroupEntity.ShutdownScheduleId.ToString();
            chkPreventShutdown.Checked = GroupEntity.PreventShutdown;
            
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            GroupEntity.Name = txtName.Text;
            GroupEntity.Description = txtDescription.Text;
            GroupEntity.ClusterId = Convert.ToInt32(ddlCluster.SelectedValue);
            GroupEntity.WakeupScheduleId = Convert.ToInt32(ddlWakeup.SelectedValue);
            GroupEntity.ShutdownScheduleId = Convert.ToInt32(ddlShutdown.SelectedValue);
            GroupEntity.PreventShutdown = chkPreventShutdown.Checked;
            var result = Call.GroupApi.Put(GroupEntity.Id, GroupEntity);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Group {0}", GroupEntity.Name) : result.ErrorMessage;
        }



        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.GroupApi.Delete(GroupEntity.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Group {0}", GroupEntity.Name);
                Response.Redirect("~/views/groups/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}