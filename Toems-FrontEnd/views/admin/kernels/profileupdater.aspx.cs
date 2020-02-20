using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;

namespace Toems_FrontEnd.views.admin.kernels
{
    public partial class profileupdater : BasePages.Admin
    {
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvProfiles);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateGrid();
            }
        }

        protected void PopulateGrid()
        {
            ddlKernel.DataSource = Call.FilesystemApi.GetKernels();
            ddlKernel.DataBind();
            ddlKernel.SelectedValue = SettingStrings.DefaultKernel64;
            gvProfiles.DataSource = Call.ImageProfileApi.Get();
            gvProfiles.DataBind();
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            var updateCount = 0;
            foreach (GridViewRow row in gvProfiles.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvProfiles.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

                var imageProfile = Call.ImageProfileApi.Get(Convert.ToInt32(dataKey.Value));
                imageProfile.Kernel = ddlKernel.Text;
                if (Call.ImageProfileApi.Put(imageProfile.Id, imageProfile).Success)
                {
                    updateCount++;
                }
            }
            EndUserMessage = "Updated " + updateCount + " Image Profile(s)";
            PopulateGrid();
        }
    }
}