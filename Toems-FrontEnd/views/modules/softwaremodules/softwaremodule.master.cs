using System;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.modules.softwaremodules
{
    public partial class software : BasePages.MasterBaseMaster
    {
        public EntitySoftwareModule SoftwareModule { get; set; }
        private BasePages.Modules ModulesBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ModulesBasePage = Page as BasePages.Modules;
            SoftwareModule = ModulesBasePage.SoftwareModule;
            if (SoftwareModule != null)
            {
                Level1.Visible = false;
                Level2.Visible = true;
            }
            else
            {
                Level2.Visible = false;
                btnDelete.Visible = false;
                btnArchive.Visible = false;
            }
          
        }

        protected void btnArchive_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Archive " + SoftwareModule.Name + "?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + SoftwareModule.Name + "?";
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
                    result = ModulesBasePage.Call.SoftwareModuleApi.Delete(SoftwareModule.Id);
                    actionLabel = "Deleted";
                    break;
                case "archive":
                    result = ModulesBasePage.Call.ModuleApi.Archive(SoftwareModule.Id, EnumModule.ModuleType.Software);
                    actionLabel = "Archived";
                    break;
            }


            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully " + actionLabel + " Module: " + SoftwareModule.Name;
                Response.Redirect("~/views/modules/softwaremodules/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;         
        }
    }
}