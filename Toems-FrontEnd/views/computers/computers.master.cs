using System;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.computers
{
    public partial class computers : BasePages.MasterBaseMaster
    {
        public EntityComputer ComputerEntity { get; set; }
        private BasePages.Computers ComputerBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ComputerBasePage = Page as BasePages.Computers;
            ComputerEntity = ComputerBasePage.ComputerEntity;
            if (ComputerEntity == null) //level 2
            {
                Level2.Visible = false;
                btnArchive.Visible = false;
                btnDelete.Visible = false;
                btnCheckin.Visible = false;
                btnUsers.Visible = false;
                btnStatus.Visible = false;
                btnReboot.Visible = false;
                btnShutdown.Visible = false;
                btnWakeup.Visible = false;
                btnInventory.Visible = false;
                btnDeploy.Visible = false;
                btnUpload.Visible = false;

            }
            else
            {
                Level1.Visible = false;
                btnArchive.Visible = true;
                btnDelete.Visible = true; 
                btnCheckin.Visible = true;
                btnUsers.Visible = true;
                btnStatus.Visible = true;
                btnReboot.Visible = true;
                btnShutdown.Visible = true;
                btnWakeup.Visible = true;
                btnInventory.Visible = true;
                btnDeploy.Visible = true;
                btnUpload.Visible = true;

            }
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            var action = (string) Session["action"];
            Session.Remove("action");
            
            var result = new DtoActionResult();
            var actionLabel = string.Empty;
            switch (action)
            {
                case "delete":
                    result = ComputerBasePage.Call.ComputerApi.Delete(ComputerEntity.Id);
                    actionLabel = "Deleted";
                    break;
                case "archive":
                    result = ComputerBasePage.Call.ComputerApi.Archive(ComputerEntity.Id);
                    actionLabel = "Archived";
                    break;
                case "reboot":
                    ComputerBasePage.RequiresAuthorization(AuthorizationStrings.ComputerReboot);
                    ComputerBasePage.Call.ComputerApi.Reboot(ComputerEntity.Id);
                    actionLabel = "Rebooted";
                    result = new DtoActionResult() {Success = true};
                    break;
                case "shutdown":
                    ComputerBasePage.RequiresAuthorization(AuthorizationStrings.ComputerShutdown);
                    ComputerBasePage.Call.ComputerApi.Shutdown(ComputerEntity.Id);
                    actionLabel = "Shutdown";
                    result = new DtoActionResult() { Success = true };
                    break;
                case "wakeup":
                    ComputerBasePage.RequiresAuthorization(AuthorizationStrings.ComputerWakeup);
                    ComputerBasePage.Call.ComputerApi.Wakeup(ComputerEntity.Id);
                    actionLabel = "Sent Wakeup Packet To";
                    result = new DtoActionResult() { Success = true };
                    break;
                case "deploy":
                    PageBaseMaster.EndUserMessage = ComputerBasePage.Call.ComputerApi.StartDeploy(ComputerEntity.Id);
                    break;
                case "upload":
                    PageBaseMaster.EndUserMessage = ComputerBasePage.Call.ComputerApi.StartUpload(ComputerEntity.Id);
                    break;
            }

            if (action.Equals("deploy") || action.Equals("upload"))
                return;

            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully " + actionLabel + " Computer: " + ComputerEntity.Name;
                if(action.Equals("delete") || action.Equals("archive"))
                Response.Redirect("~/views/computers/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;         
        }

        protected void btnArchive_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Archive " + ComputerEntity.Name + "?";
            Session["action"] = "archive";
            DisplayConfirm();

        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + ComputerEntity.Name + "?";
            Session["action"] = "delete";
            DisplayConfirm();

        }

        protected void btnCheckin_OnClick(object sender, EventArgs e)
        {
            ComputerBasePage.RequiresAuthorization(AuthorizationStrings.ComputerForceCheckin);
            ComputerBasePage.Call.ComputerApi.ForceCheckin(ComputerEntity.Id);
            PageBaseMaster.EndUserMessage = "Force Checkin Request Sent";
        }

        protected void btnInventory_OnClick(object sender, EventArgs e)
        {
            ComputerBasePage.RequiresAuthorization(AuthorizationStrings.ComputerForceCheckin);
            ComputerBasePage.Call.ComputerApi.CollectInventory(ComputerEntity.Id);
            PageBaseMaster.EndUserMessage = "Inventory Request Sent";
        }

        protected void btnUsers_OnClick(object sender, EventArgs e)
        {
            var users = ComputerBasePage.Call.ComputerApi.GetLoggedInUsers(ComputerEntity.Id);
            var replaced = users.Replace("\\", "\\\\");
            PageBaseMaster.EndUserMessage = "Logged In Users: " + replaced;
        }

        protected void btnStatus_OnClick(object sender, EventArgs e)
        {
            var status = ComputerBasePage.Call.ComputerApi.GetStatus(ComputerEntity.Id);
            PageBaseMaster.EndUserMessage = "Status: " + status;
        }

        protected void btnReboot_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Reboot " + ComputerEntity.Name + "?";
            Session["action"] = "reboot";
            DisplayConfirm();        
        }

        protected void btnShutdown_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Shutdown " + ComputerEntity.Name + "?";
            Session["action"] = "shutdown";
            DisplayConfirm();
          
        }

        protected void btnWakeup_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Upload Image From " + ComputerEntity.Name + "?";
            Session["action"] = "upload";
            DisplayConfirm();

        }

        protected void btnDeploy_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Deploy Image To " + ComputerEntity.Name + "?";
            Session["action"] = "deploy";
            DisplayConfirm();

        }
    }
}