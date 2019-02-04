using System;

namespace Toems_FrontEnd.views.computers
{
    public partial class usages : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        private void PopulateForm()
        {
            gvModules.DataSource = null;
            gvModules.DataBind();
            gvGroups.DataSource = null;
            gvGroups.DataBind();
            gvPolicies.DataSource = null;
            gvPolicies.DataBind();
            gvPolicies.Visible = false;
            gvGroups.Visible = false;
            gvModules.Visible = false;

            if (ddlUtil.Text == "Groups")
            {
                gvGroups.Visible = true;
                gvGroups.DataSource = Call.ComputerApi.GetComputerGroups(ComputerEntity.Id);
                gvGroups.DataBind();
            }
            else if (ddlUtil.Text == "Policies")
            {
                gvPolicies.Visible = true;
                gvPolicies.DataSource = Call.ComputerApi.GetComputerPolicies(ComputerEntity.Id);
                gvPolicies.DataBind();
            }
            else
            {
                gvModules.Visible = true;
                gvModules.DataSource = Call.ComputerApi.GetComputerModules(ComputerEntity.Id);
                gvModules.DataBind();
            }
        }

        protected void ddlUtil_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateForm();
        }
    }
}