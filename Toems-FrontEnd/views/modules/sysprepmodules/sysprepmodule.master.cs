using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.modules.sysprepmodules
{
    public partial class sysprepmodule : BasePages.MasterBaseMaster
    {
        public EntitySysprepModule SysprepModule { get; set; }
        private Modules ModulesBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ModulesBasePage = Page as Modules;
            SysprepModule = ModulesBasePage.SysprepModule;
            if (SysprepModule != null)
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
            lblTitle.Text = "Archive " + SysprepModule.Name + "?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + SysprepModule.Name + "?";
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
                    result = ModulesBasePage.Call.SysprepModuleApi.Delete(SysprepModule.Id);
                    actionLabel = "Deleted";
                    break;
                case "archive":
                    result = ModulesBasePage.Call.ModuleApi.Archive(SysprepModule.Id, EnumModule.ModuleType.Sysprep);
                    actionLabel = "Archived";
                    break;
            }


            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully " + actionLabel + " Module: " + SysprepModule.Name;
                Response.Redirect("~/views/modules/sysprepmodules/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }
    }
}