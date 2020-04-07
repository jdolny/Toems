using System;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.groups
{
    public partial class groups : BasePages.MasterBaseMaster
    {
        public EntityGroup GroupEntity { get; set; }
        private BasePages.Groups GroupBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            GroupBasePage = Page as BasePages.Groups;
            GroupEntity = GroupBasePage.GroupEntity;
            if (GroupEntity == null) 
            {
                Level2.Visible = false;
                btnCheckin.Visible = false;
                btnDelete.Visible = false;
                btnInventory.Visible = false;
                btnReboot.Visible = false;
                btnShutdown.Visible = false;
                btnWakeup.Visible = false;
                btnPin.Visible = false;
                btnUnpin.Visible = false;
                btnMulticast.Visible = false;
                btnUnicast.Visible = false;
                btnClearImagingId.Visible = false;

            }
            else
            {
                Level1.Visible = false;
                btnCheckin.Visible = true;
                btnDelete.Visible = true;
                btnInventory.Visible = true;
                btnReboot.Visible = true;
                btnShutdown.Visible = true;
                btnWakeup.Visible = true;
                btnPin.Visible = true;
                btnUnpin.Visible = true;
                btnMulticast.Visible = true;
                btnUnicast.Visible = true;
                btnClearImagingId.Visible = true;
                if (GroupEntity.Type == "Dynamic")
                {
                    dynamic.Visible = true;
                    addmembers.Visible = false;
                }
                else if (GroupEntity.IsOu)
                {
                    dynamic.Visible = false;
                    addmembers.Visible = false;
                }
                else
                {
                    dynamic.Visible = false;
                    addmembers.Visible = true;
                }
            }
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            var action = (string)Session["action"];
            Session.Remove("action");

            var result = new DtoActionResult();
            var actionLabel = string.Empty;
            switch (action)
            {
                case "clearImagingId":
                    result = GroupBasePage.Call.GroupApi.ClearImagingIds(GroupEntity.Id);
                    actionLabel = "Cleared Imaging Ids For";
                    break;
                case "delete":
                    result = GroupBasePage.Call.GroupApi.Delete(GroupEntity.Id);
                    actionLabel = "Deleted";
                    break;
                case "reboot":
                    GroupBasePage.RequiresAuthorization(AuthorizationStrings.GroupReboot);
                    GroupBasePage.Call.GroupApi.Reboot(GroupEntity.Id);
                    result = new DtoActionResult() {Success = true};
                    actionLabel = "Sent Reboot Request To";
                    break;
                case "shutdown":
                    GroupBasePage.RequiresAuthorization(AuthorizationStrings.GroupShutdown);
                    GroupBasePage.Call.GroupApi.Shutdown(GroupEntity.Id);
                    result = new DtoActionResult() { Success = true };
                    actionLabel = "Sent Shutdown Request To";
                    break;
                case "wakeup":
                    GroupBasePage.RequiresAuthorization(AuthorizationStrings.GroupWakeup);
                    GroupBasePage.Call.GroupApi.Wakeup(GroupEntity.Id);
                    result = new DtoActionResult() { Success = true };
                    actionLabel = "Sent Wake Up Request To";
                    break;
                case "unicast":
                    var successCount = GroupBasePage.Call.GroupApi.StartGroupUnicast(GroupEntity.Id);
                    PageBaseMaster.EndUserMessage = "Successfully Started " + successCount + " Tasks";
                    break;
                case "multicast":
                    PageBaseMaster.EndUserMessage = GroupBasePage.Call.GroupApi.StartMulticast(GroupEntity.Id);
                    break;
            }

            if (action.Equals("unicast") || action.Equals("multicast"))
                return;

            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully " + actionLabel + " Group: " + GroupEntity.Name;
                if(action.Equals("delete"))
                Response.Redirect("~/views/groups/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;         
        }

        protected void btnCheckin_OnClick(object sender, EventArgs e)
        {
            GroupBasePage.RequiresAuthorization(AuthorizationStrings.ComputerForceCheckin);
            GroupBasePage.Call.GroupApi.ForceCheckin(GroupEntity.Id);
            PageBaseMaster.EndUserMessage = "Successfully Sent Force Checkin Request To Group " + GroupEntity.Name;
        }

        protected void btnInventory_OnClick(object sender, EventArgs e)
        {
            GroupBasePage.RequiresAuthorization(AuthorizationStrings.ComputerForceCheckin);
            GroupBasePage.Call.GroupApi.CollectInventory(GroupEntity.Id);
            PageBaseMaster.EndUserMessage = "Successfully Sent Inventory Request To Group " + GroupEntity.Name;
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + GroupEntity.Name + "?";
            Session["action"] = "delete";
            DisplayConfirm();

        }

        protected void btnReboot_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Reboot " + GroupEntity.Name + "?";
            Session["action"] = "reboot";
            DisplayConfirm();

            
          
        }

        protected void btnShutdown_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Shutdown " + GroupEntity.Name + "?";
            Session["action"] = "shutdown";
            DisplayConfirm(); 
        }

        protected void btnWakeup_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Wake Up " + GroupEntity.Name + "?";
            Session["action"] = "wakeup";
            DisplayConfirm();
        }

        protected void btnPin_OnClick(object sender, EventArgs e)
        {
            var pinnedPolicy = new EntityPinnedGroup();
            pinnedPolicy.GroupId = GroupEntity.Id;
            pinnedPolicy.UserId = GroupBasePage.ToemsCurrentUser.Id;
            var result = GroupBasePage.Call.PinnedGroupApi.Post(pinnedPolicy);
            if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Pinned Group " + GroupEntity.Name;
            else PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }

        protected void btnUnpin_OnClick(object sender, EventArgs e)
        {
            var result = GroupBasePage.Call.PinnedGroupApi.Delete(GroupEntity.Id, GroupBasePage.ToemsCurrentUser.Id);
            if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Unpinned Group " + GroupEntity.Name;
            else PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }

        protected void btnMulticast_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "Multicast " + GroupEntity.Name + "?";
            Session["action"] = "multicast";
            DisplayConfirm();
        }

        protected void btnUnicast_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "Unicast " + GroupEntity.Name + "?";
            Session["action"] = "unicast";
            DisplayConfirm();
        }

        protected void btnClearImagingId_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "Clear Client Imaging Ids For " + GroupEntity.Name + "?";
            Session["action"] = "clearImagingId";
            DisplayConfirm();
        }
    }
}