using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.modules.winpemodules
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
            gvComputers.DataSource = null;
            gvComputers.DataBind();
            gvGroups.DataSource = null;
            gvGroups.DataBind();
            gvPolicies.DataSource = null;
            gvPolicies.DataBind();
     
            gvPolicies.Visible = false;
            gvGroups.Visible = false;
            gvComputers.Visible = false;


            if (ddlUtil.Text == "Computers")
            {
                gvComputers.Visible = true;
                gvComputers.DataSource = Call.ModuleApi.GetModuleComputers(WinPeModule.Guid);
                gvComputers.DataBind();
            }
            else if (ddlUtil.Text == "Policies")
            {
                gvPolicies.Visible = true;
                gvPolicies.DataSource = Call.ModuleApi.GetModulePolicies(WinPeModule.Guid);
                gvPolicies.DataBind();
            }
            else
            {
                gvGroups.Visible = true;
                gvGroups.DataSource = Call.ModuleApi.GetModuleGroups(WinPeModule.Guid);
                gvGroups.DataBind();
            }
        }

        protected void ddlUtil_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateForm();
        }
    }
}