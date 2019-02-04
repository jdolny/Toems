using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.assets.customassets
{
    public partial class attachments : BasePages.Assets
    {
        protected string token { get; set; }
        protected string baseurl { get; set; }
        protected string attachmentGuid { get; set; }
        protected string assetId { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.AttachmentRead);
            if (!IsPostBack)
            {
                token = Request.Cookies["toemsToken"].Value;
                baseurl = ConfigurationManager.AppSettings["UploadApiUrl"];
                if (!baseurl.EndsWith("/"))
                    baseurl = baseurl + "/";
                attachmentGuid = Guid.NewGuid().ToString();
                assetId = Asset.Id.ToString();
                BindGrid();
            }

        }
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvAttachments);
        }

        protected void UploadButton_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.AttachmentAdd);
            EndUserMessage = "Complete";
            BindGrid();
        }

        protected void ErrorButton_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.AttachmentAdd);
            EndUserMessage = uploadErrorMessage.Value;
            Response.Redirect("~/views/assets/customassets/attachments.aspx?level=2&assetId=" + Asset.Id);
        }

        private void BindGrid()
        {
            gvAttachments.DataSource = Call.AssetApi.GetAttachments(Asset.Id);
            gvAttachments.DataBind();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete The Selected Attachments?";
            DisplayConfirm();
        }

        protected void btnDownload_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.AttachmentAdd);
            var control = sender as Control;
            var attachment = new EntityAttachment();
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvAttachments.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    attachment = Call.AttachmentApi.Get(Convert.ToInt32(dataKey.Value));

                }
            }

            Response.Clear();
            var ms = new MemoryStream(Call.AttachmentApi.GetAttachment(attachment.Id));

            Response.ContentType = "application/octet-stream";
            Response.AddHeader("content-disposition", "attachment;filename=" + "\"" + attachment.Name + "\"");

            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }

        protected void ConfirmButton_OnClick(object sender, EventArgs e)
        {
            var count = 0;
            foreach (GridViewRow row in gvAttachments.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvAttachments.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.AttachmentApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                    count++;
            }
            EndUserMessage = "Deleted " + count + " Attachment(s)";
            BindGrid();
        }
    }
}