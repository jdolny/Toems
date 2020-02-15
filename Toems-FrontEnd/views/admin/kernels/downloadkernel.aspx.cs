using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.admin.kernels
{
    public partial class downloadkernel : BasePages.Admin
    {
        protected void btnDownload_OnClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                var onlineKernel = new DtoOnlineKernel();
                var gvRow = (GridViewRow)control.Parent.Parent;
                onlineKernel.BaseVersion = gvRow.Cells[2].Text;
                onlineKernel.FileName = gvRow.Cells[0].Text;
                if (onlineKernel.BaseVersion != null)
                {
                    var result = Call.OnlineKernelApi.DownloadKernel(onlineKernel);
                    EndUserMessage = result ? "Successfully Downloaded Kernel" : "Could Not Download Kernel";
                }
            }
            PopulateKernels();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateKernels();
        }

        private void PopulateKernels()
        {
            var onlineKernels = Call.OnlineKernelApi.Get();
            if (onlineKernels == null)
            {
                EndUserMessage = "Could Not Get Online Kernel List.  Internet Access Is Required.";
                return;
            }
            gvKernels.DataSource = onlineKernels;
            gvKernels.DataBind();

            var installedKernels = Call.FilesystemApi.GetKernels();
            foreach (GridViewRow row in gvKernels.Rows)
            {
                var lbl = row.FindControl("lblInstalled") as Label;
                foreach (var kernel in installedKernels)
                {
                    if (kernel == row.Cells[0].Text && lbl != null)
                        lbl.Text = "Yes";
                }
            }
        }
    }
}