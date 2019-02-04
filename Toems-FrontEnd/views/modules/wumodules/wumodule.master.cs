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

namespace Toems_FrontEnd.views.modules.wumodules
{
    public partial class wumodule : MasterBaseMaster
    {
        public EntityWuModule WuModule { get; set; }
        private BasePages.Modules ModulesBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ModulesBasePage = Page as BasePages.Modules;
            WuModule = ModulesBasePage.WuModule;
            if (WuModule != null)
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
            lblTitle.Text = "Archive " + WuModule.Name + "?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + WuModule.Name + "?";
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
                    result = ModulesBasePage.Call.WuModuleApi.Delete(WuModule.Id);
                    actionLabel = "Deleted";
                    break;
                case "archive":
                    result = ModulesBasePage.Call.ModuleApi.Archive(WuModule.Id, EnumModule.ModuleType.Wupdate);
                    actionLabel = "Archived";
                    break;
            }


            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully " + actionLabel + " Module: " + WuModule.Name;
                Response.Redirect("~/views/modules/wumodules/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }
    }
}