using System;

namespace Toems_FrontEnd.views.modules.scriptmodules
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
            gvImages.DataSource = null;
            gvImages.DataBind();
            gvImages.Visible = false;
            gvPolicies.Visible = false;
            gvGroups.Visible = false;
            gvComputers.Visible = false;

            if (ddlUtil.Text == "Computers")
            {
                gvComputers.Visible = true;
                gvComputers.DataSource = Call.ModuleApi.GetModuleComputers(ScriptModule.Guid);
                gvComputers.DataBind();
            }
            else if (ddlUtil.Text == "Policies")
            {
                gvPolicies.Visible = true;
                gvPolicies.DataSource = Call.ModuleApi.GetModulePolicies(ScriptModule.Guid);
                gvPolicies.DataBind();
            }
            else if (ddlUtil.Text == "Images")
            {
                gvImages.Visible = true;
                gvImages.DataSource = Call.ModuleApi.GetModuleImages(ScriptModule.Guid);
                gvImages.DataBind();
            }
            else
            {
                gvGroups.Visible = true;
                gvGroups.DataSource = Call.ModuleApi.GetModuleGroups(ScriptModule.Guid);
                gvGroups.DataBind();
            }
        }

        protected void ddlUtil_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateForm();
        }
    }
}