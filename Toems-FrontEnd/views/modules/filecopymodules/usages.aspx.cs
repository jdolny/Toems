using System;

namespace Toems_FrontEnd.views.modules.filecopymodules
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
                gvComputers.DataSource = Call.ModuleApi.GetModuleComputers(FileCopyModule.Guid);
                gvComputers.DataBind();
            }
            else if (ddlUtil.Text == "Policies")
            {
                gvPolicies.Visible = true;
                gvPolicies.DataSource = Call.ModuleApi.GetModulePolicies(FileCopyModule.Guid);
                gvPolicies.DataBind();
            }
            else
            {
                gvGroups.Visible = true;
                gvGroups.DataSource = Call.ModuleApi.GetModuleGroups(FileCopyModule.Guid);
                gvGroups.DataBind();
            }
        }

        protected void ddlUtil_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateForm();
        }
    }
}