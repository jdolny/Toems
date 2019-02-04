using System;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.modules.scriptmodules
{
    public partial class scriptmodule : BasePages.MasterBaseMaster
    {
        public EntityScriptModule ScriptModule { get; set; }
        private BasePages.Modules ModulesBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ModulesBasePage = Page as BasePages.Modules;
            ScriptModule = ModulesBasePage.ScriptModule;
            if (ScriptModule != null)
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
            lblTitle.Text = "Archive " + ScriptModule.Name + "?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + ScriptModule.Name + "?";
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
                    result = ModulesBasePage.Call.ScriptModuleApi.Delete(ScriptModule.Id);
                    actionLabel = "Deleted";
                    break;
                case "archive":
                    result = ModulesBasePage.Call.ModuleApi.Archive(ScriptModule.Id, EnumModule.ModuleType.Script);
                    actionLabel = "Archived";
                    break;
            }


            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully " + actionLabel + " Module: " + ScriptModule.Name;
                Response.Redirect("~/views/modules/scriptmodules/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }
    }
}