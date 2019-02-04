using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Toems_Common.Dto.exports;

namespace Toems_FrontEnd.views.policies
{
    public partial class import : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnImport_OnClick(object sender, EventArgs e)
        {
            if (FileImport.HasFile)
            {
                string json;
                using (var inputStreamReader = new StreamReader(FileImport.PostedFile.InputStream))
                {
                    json = inputStreamReader.ReadToEnd();
                }

                var policy = new DtoPolicyExport();
                try
                {
                    policy = JsonConvert.DeserializeObject<DtoPolicyExport>(json);
                }
                catch
                {
                    EndUserMessage = "Could Not Parse JSON";
                    return;
                }

                var result = Call.PolicyApi.ImportPolicy(policy);

                if (result == null)
                {
                    EndUserMessage = "Policy Import Failed.  Unknown Error.";
                    return;
                }

                if (!result.Success)
                {
                    EndUserMessage = "Policy Import Failed.  " + result.ErrorMessage;
                    return;
                }

                EndUserMessage = "Successfully Imported Policy.  ";

                if (result.HasInternalFiles)
                {
                    EndUserMessage += "This Policy Has Additional Files That Must Be Uploaded Prior To Use.  ";
                }

                if (result.HasExternalFiles)
                {
                    EndUserMessage +=
                        "This Policy Has External Files That Have Been Queued For Download.  Policy Cannot Be Activated Until All Downloads Are Completed.";
                }

            }
        }
    }
}