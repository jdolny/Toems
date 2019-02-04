using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin
{
    public partial class certificates : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        private void PopulateForm()
        {
            gvCertificates.DataSource = Call.SettingApi.GetCAInt();
            gvCertificates.DataBind();
        }

        protected void btnGenerate_OnClick(object sender, EventArgs e)
        {
            var result = Call.SettingApi.GenerateCAInt();
            EndUserMessage = result ? "Successfully Generated Certificates." : "Could Not Generate Certificates";

            PopulateForm();
        }

        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow) control.Parent.Parent;
                var dataKey = gvCertificates.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var cert = Call.SettingApi.ExportCert(Convert.ToInt32(dataKey.Value));

                    Response.Clear();
                    var ms = new MemoryStream(cert);

                    Response.ContentType = "application/x-x509-ca-cert";
                    Response.AddHeader("content-disposition", "attachment;filename=Certificate.cer");


                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();
                }
            }

        }
    }
}