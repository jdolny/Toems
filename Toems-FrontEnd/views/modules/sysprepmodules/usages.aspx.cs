using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.modules.sysprepmodules
{
    public partial class usages : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        private void PopulateForm()
        {
            gvImages.DataSource = null;
            gvImages.DataBind();
            gvImages.Visible = false;

            gvImages.Visible = true;
            gvImages.DataSource = Call.ModuleApi.GetModuleImages(SysprepModule.Guid);
            gvImages.DataBind();

        }

        protected void ddlUtil_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateForm();
        }
    }
}