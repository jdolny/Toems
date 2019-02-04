using System;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.printermodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            ddlType.DataSource = Enum.GetNames(typeof(EnumPrinterModule.ActionType));
            ddlType.DataBind();

            txtDisplayName.Text = PrinterModule.Name;
            txtDescription.Text = PrinterModule.Description;
            ddlType.SelectedValue = PrinterModule.Action.ToString();
            txtPath.Text = PrinterModule.NetworkPath;
            chkDefault.Checked = PrinterModule.IsDefault;
            chkRestartSpooler.Checked = PrinterModule.RestartSpooler;
            chkEnumeration.Checked = PrinterModule.WaitForEnumeration;


        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (PrinterModule.Archived)
            {
                EndUserMessage = "Archived Modules Cannot Be Modified";
                return;
            }
            PrinterModule.Name = txtDisplayName.Text;
            PrinterModule.Description = txtDescription.Text;
            PrinterModule.Action =
                (EnumPrinterModule.ActionType) Enum.Parse(typeof (EnumPrinterModule.ActionType), ddlType.SelectedValue);
            PrinterModule.NetworkPath = txtPath.Text;
            PrinterModule.IsDefault = chkDefault.Checked;
            PrinterModule.RestartSpooler = chkRestartSpooler.Checked;
            PrinterModule.WaitForEnumeration = chkEnumeration.Checked;

            var result = Call.PrinterModuleApi.Put(PrinterModule.Id, PrinterModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}",PrinterModule.Name) : result.ErrorMessage;
        }

     

        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.PrinterModuleApi.Delete(PrinterModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}",PrinterModule.Name);
                Response.Redirect("~/views/modules/printermodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}