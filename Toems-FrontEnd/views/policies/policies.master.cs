using System;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.policies
{
    public partial class policies : MasterBaseMaster
    {
        public EntityPolicy Policy { get; set; }
        private BasePages.Policies PolicyBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            PolicyBasePage = Page as BasePages.Policies;
            Policy = PolicyBasePage.Policy;
            if (Policy == null) //level 1
            {
                Level2.Visible = false;
                btnDelete.Visible = false;
                btnActivate.Visible = false;
                btnArchive.Visible = false;
                btnDeactivate.Visible = false;
                btnPin.Visible = false;
                btnUnpin.Visible = false;

            }
            else
            {
                Level1.Visible = false;
                btnDelete.Visible = true;
                btnActivate.Visible = true;
                btnArchive.Visible = true;
                btnDeactivate.Visible = true;
                btnPin.Visible = true;
                btnUnpin.Visible = true;

            }
        }
        protected void btnArchive_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Archive " + Policy.Name + "?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + Policy.Name + "?";
            Session["action"] = "delete";
            DisplayConfirm();
        }

        protected void buttonConfirm_OnClick(object sender, EventArgs e)
        {
            var action = (string)Session["action"];
            Session.Remove("action");

            var result = new DtoActionResult();
            var actionLabel = string.Empty;
            switch (action)
            {
                case "delete":
                    result = PolicyBasePage.Call.PolicyApi.Delete(Policy.Id);
                    actionLabel = "Deleted";
                    break;
                case "archive":
                    result = PolicyBasePage.Call.PolicyApi.Archive(Policy.Id);
                    actionLabel = "Archived";
                    break;
            }


            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully " + actionLabel + " Policy: " + Policy.Name;
                Response.Redirect("~/views/policies/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;         
        }

        protected void btnActivate_OnClick(object sender, EventArgs e)
        {
            if (Policy.Frequency == EnumPolicy.Frequency.Ongoing)
            {
                var result = PolicyBasePage.Call.PolicyApi.ActivatePolicy(Policy.Id, false);
                if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Activated " + Policy.Name;
                else PageBaseMaster.EndUserMessage = result.ErrorMessage;
            }
            else
            {
                var policyChanged = PolicyBasePage.Call.PolicyApi.PolicyChangedSinceActivation(Policy.Id);
                if (policyChanged == "new" || policyChanged == "false")
                {
                    var result = PolicyBasePage.Call.PolicyApi.ActivatePolicy(Policy.Id, false);
                    if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Activated " + Policy.Name;
                    else PageBaseMaster.EndUserMessage = result.ErrorMessage;

                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "modalscript",
                        "$(function() {  var menuTop = document.getElementById('confirmbox2'),body = document.body;classie.toggle(menuTop, 'confirm-box-outer-open'); });",
                        true);
                }
            }
        }

        protected void btnDeactivate_OnClick(object sender, EventArgs e)
        {
            var result = PolicyBasePage.Call.PolicyApi.DeactivatePolicy(Policy.Id);
            if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Deactivated " + Policy.Name;
            else PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }

        protected void btnPin_OnClick(object sender, EventArgs e)
        {
            var pinnedPolicy = new EntityPinnedPolicy();
            pinnedPolicy.PolicyId = Policy.Id;
            pinnedPolicy.UserId = PolicyBasePage.ToemsCurrentUser.Id;
            var result = PolicyBasePage.Call.PinnedPolicyApi.Post(pinnedPolicy);
            if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Pinned Policy " + Policy.Name;
            else PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }

        protected void btnUnpin_OnClick(object sender, EventArgs e)
        {
            var result = PolicyBasePage.Call.PinnedPolicyApi.Delete(Policy.Id, PolicyBasePage.ToemsCurrentUser.Id);
            if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Unpinned Policy " + Policy.Name;
            else PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }

        protected void btnAll_OnClick(object sender, EventArgs e)
        {
            var result = PolicyBasePage.Call.PolicyApi.ActivatePolicy(Policy.Id, true);
            if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Activated " + Policy.Name;
            else PageBaseMaster.EndUserMessage = result.ErrorMessage;
           

        }

        protected void btnNewOnly_OnClick(object sender, EventArgs e)
        {
            var result = PolicyBasePage.Call.PolicyApi.ActivatePolicy(Policy.Id, false);
            if (result.Success) PageBaseMaster.EndUserMessage = "Successfully Activated " + Policy.Name;
            else PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }
    }
}