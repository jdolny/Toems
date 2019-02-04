using System;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.printermodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlType.DataSource = Enum.GetNames(typeof (EnumPrinterModule.ActionType));
                ddlType.DataBind();
            }
        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            var module = new EntityPrinterModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                Action = (EnumPrinterModule.ActionType)Enum.Parse(typeof(EnumPrinterModule.ActionType),ddlType.SelectedValue),
                NetworkPath = txtPath.Text,
                IsDefault = chkDefault.Checked,
                RestartSpooler = chkRestartSpooler.Checked,
                WaitForEnumeration = chkEnumeration.Checked
            };


            var result = Call.PrinterModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Printer Module";
                Response.Redirect("~/views/modules/printermodules/general.aspx?printerModuleId=" + result.Id);
            }
        }
    }
}