using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Toems_Common.Dto.exports;

namespace Toems_FrontEnd.views.policies
{
    public partial class export : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void buttonExport_OnClick(object sender, EventArgs e)
        {
            var exportInfo = new DtoPolicyExportGeneral();
            exportInfo.PolicyId = Policy.Id;
            exportInfo.Description = txtDescription.Text;
            exportInfo.Instructions = txtInstructions.Text;
            exportInfo.Requirements = txtRequirements.Text;
            exportInfo.Name = txtName.Text;

            var validationResult = Call.PolicyApi.ValidatePolicyExport(exportInfo);
            if (validationResult != null)
            {
                if (!validationResult.Success)
                {
                    EndUserMessage = validationResult.ErrorMessage;
                    return;
                }

                var policyExport = Call.PolicyApi.ExportPolicy(exportInfo);
                Response.ContentType = "text/plain";
                Response.AppendHeader("Content-Disposition", "attachment; filename=policy.json");
                Response.Write(JsonConvert.SerializeObject(policyExport));
                Response.End();
            }
        }
    }
}