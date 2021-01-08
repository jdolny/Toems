using System;
using System.Text;
using System.Web;
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
                btnClearImagingId.Visible = false;

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
                case "clearImagingId":
                    result = ComputerBasePage.Call.ComputerApi.ClearImagingId(ComputerEntity.Id);
                    actionLabel = "Cleared Imaging Ids For";
                    break;
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
                    ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
                    ComputerBasePage.Call.ComputerApi.Reboot(ComputerEntity.Id);
                    var counter = 0;
                    while (counter < 10)
                    {
                        var lastSocketResult = ComputerBasePage.Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                        if (!string.IsNullOrEmpty(lastSocketResult))
                        {
                            PageBaseMaster.EndUserMessage = lastSocketResult;
                            break;
                        }
                        if (counter == 9)
                        {
                            PageBaseMaster.EndUserMessage = "Could Not Reboot Computer";
                            return;
                        }
                        System.Threading.Thread.Sleep(1000);
                        counter++;
                    }
                    result = new DtoActionResult() {Success = true};
                    break;
                case "shutdown":
                    ComputerBasePage.RequiresAuthorization(AuthorizationStrings.ComputerShutdown);
                    ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
                    ComputerBasePage.Call.ComputerApi.Shutdown(ComputerEntity.Id);
                    var counter1 = 0;
                    while (counter1 < 10)
                    {
                        var lastSocketResult = ComputerBasePage.Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                        if (!string.IsNullOrEmpty(lastSocketResult))
                        {
                            PageBaseMaster.EndUserMessage = lastSocketResult;
                            break;
                        }
                        if (counter1 == 9)
                        {
                            PageBaseMaster.EndUserMessage = "Could Not Shutdown Computer";
                            return;
                        }
                        System.Threading.Thread.Sleep(1000);
                        counter1++;
                    }
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
            ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
            ComputerBasePage.Call.ComputerApi.ForceCheckin(ComputerEntity.Id);
            var counter = 0;
            while (counter < 10)
            {
                var lastSocketResult = ComputerBasePage.Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                if (!string.IsNullOrEmpty(lastSocketResult))
                {
                    PageBaseMaster.EndUserMessage = lastSocketResult;
                    break;
                }
                if (counter == 9)
                {
                    PageBaseMaster.EndUserMessage = "Could Not Run Checkin";
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }
        }

        protected void btnInventory_OnClick(object sender, EventArgs e)
        {
            ComputerBasePage.RequiresAuthorization(AuthorizationStrings.ComputerForceCheckin);
            ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
            ComputerBasePage.Call.ComputerApi.CollectInventory(ComputerEntity.Id);
            var counter = 0;
            while (counter < 10)
            {
                var lastSocketResult = ComputerBasePage.Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                if (!string.IsNullOrEmpty(lastSocketResult))
                {
                    PageBaseMaster.EndUserMessage = lastSocketResult;
                    break;
                }
                if (counter == 9)
                {
                    PageBaseMaster.EndUserMessage = "Could Not Run Inventory";
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }
        }

        protected void btnUsers_OnClick(object sender, EventArgs e)
        {
            ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
            ComputerBasePage.Call.ComputerApi.GetLoggedInUsers(ComputerEntity.Id);

            var counter = 0;
            while (counter < 10)
            {
                var lastSocketResult = ComputerBasePage.Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                if (!string.IsNullOrEmpty(lastSocketResult))
                {
                    PageBaseMaster.EndUserMessage = lastSocketResult.Replace("\\\\", "\\");
                    break;
                }
                if (counter == 9)
                {
                    PageBaseMaster.EndUserMessage = "Could Not Determine Logged On Users";
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }
        }

        protected void btnStatus_OnClick(object sender, EventArgs e)
        {
            ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
            ComputerBasePage.Call.ComputerApi.GetStatus(ComputerEntity.Id);

            var counter = 0;
            while (counter < 10)
            {
                var lastSocketResult = ComputerBasePage.Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                if (!string.IsNullOrEmpty(lastSocketResult))
                {
                    PageBaseMaster.EndUserMessage = lastSocketResult;
                    break;
                }
                if (counter == 9)
                {
                    PageBaseMaster.EndUserMessage = "Disconnected";
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }
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
            lblTitle.Text = "Wake Up " + ComputerEntity.Name + "?";
            Session["action"] = "wakeup";
            DisplayConfirm();

        }

        protected void btnUpload_OnClick(object sender, EventArgs e)
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

        protected void btnClearImagingId_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "Clear Client Imaging Id For " + ComputerEntity.Name + "?";
            Session["action"] = "clearImagingId";
            DisplayConfirm();
        }

        protected void btnRemoteControl_Click(object sender, EventArgs e)
        {
            ComputerBasePage.RequiresAuthorization(AuthorizationStrings.AllowRemoteControl);
            if (string.IsNullOrEmpty(ComputerEntity.RemoteAccessId))
            {
                PageBaseMaster.EndUserMessage = "Cannot Start Remote Control.  The Remote Control Agent Is Not Installed On This Computer.";
                return;
            }
            ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);

            ComputerBasePage.Call.ComputerApi.StartRemoteControl(ComputerEntity.Id);

            var counter = 0;
            while(counter < 10)
            {
                var lastSocketResult = ComputerBasePage.Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                if(!string.IsNullOrEmpty(lastSocketResult))
                {
                    if (lastSocketResult.Contains("Error"))
                    {
                        PageBaseMaster.EndUserMessage = lastSocketResult;
                        return;
                    }
                    if (lastSocketResult.Equals("Ready"))
                        break;
                }
                if (counter == 9)
                {
                    PageBaseMaster.EndUserMessage = "Remote Access Is Not Ready";
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }

            //give device time to come online
            counter = 0;
            while (counter < 10)
            {
                var isOnline = ComputerBasePage.Call.RemoteAccessApi.IsDeviceOnline(ComputerEntity.RemoteAccessId);
                if(isOnline != null)
                {
                    if (isOnline.Equals("true"))
                        break;
                }
                if(counter == 9)
                {
                    PageBaseMaster.EndUserMessage = "Device Is Not Online";
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }

            var url = ComputerBasePage.Call.RemoteAccessApi.GetRemoteControlUrl(ComputerEntity.RemoteAccessId);
            if (string.IsNullOrEmpty(url))
                PageBaseMaster.EndUserMessage = "Unknown Error.  Check The Exception Logs.";
            else if (url.Contains("Error"))
                PageBaseMaster.EndUserMessage = url;
            else if (url.Contains("http") || url.Contains("https"))
            {
                PageBaseMaster.EndUserMessage = "Started Remote Control";
                string redirect = $"<script>window.open('{url}');</script>";
                Response.Write(redirect);
            }

        }

        protected void btnServiceLog_Click(object sender, EventArgs e)
        {
            ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
            ComputerBasePage.Call.ComputerApi.GetServiceLog(ComputerEntity.Id);

            var counter = 0;
            var lastSocketResult = "";
            while (counter < 10)
            {
                lastSocketResult = ComputerBasePage.Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                if (!string.IsNullOrEmpty(lastSocketResult))
                {
                    break;
                }
                if (counter == 9)
                {
                    PageBaseMaster.EndUserMessage = "Could Not Retrieve Log";
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }

            ComputerBasePage.Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
            if (!string.IsNullOrEmpty(lastSocketResult))
            {
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AppendHeader("Content-Disposition",
                    "attachment; filename=service.log");

                HttpContext.Current.Response.Write(lastSocketResult);
                HttpContext.Current.Response.End();
            }
        }
    }
}