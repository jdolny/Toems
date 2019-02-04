using System;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.groups
{
    public partial class create : BasePages.Groups
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                PopulateClusterGroupsDdl(ddlCluster);
        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            var group = new EntityGroup();
            group.Name = txtName.Text;
            group.Description = txtDescription.Text;
            group.Type = ddlGroupType.Text;
            group.ClusterId = Convert.ToInt32(ddlCluster.SelectedValue);
            group.PreventShutdown = chkPreventShutdown.Checked;

            var result = Call.GroupApi.Post(group);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Group";
                Response.Redirect("~/views/groups/general.aspx?groupId=" + result.Id);
            }
        }
    }
}