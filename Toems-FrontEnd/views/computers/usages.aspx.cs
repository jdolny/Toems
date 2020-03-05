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
            gvComServers.DataSource = null;
            gvComServers.DataBind();
            gvComServers.Visible = false;
            gvPolicies.Visible = false;
            gvGroups.Visible = false;
            gvModules.Visible = false;

            if (ddlUtil.Text == "Groups")
            {
                gvGroups.Visible = true;
                gvGroups.DataSource = Call.ComputerApi.GetComputerGroupsWithImage(ComputerEntity.Id);
                gvGroups.DataBind();
            }
            else if (ddlUtil.Text == "Policies")
            {
                gvPolicies.Visible = true;
                gvPolicies.DataSource = Call.ComputerApi.GetComputerPolicies(ComputerEntity.Id);
                gvPolicies.DataBind();
            }
            else if (ddlUtil.Text == "Endpoint Management Servers")
            {
                gvComServers.Visible = true;
                gvComServers.DataSource = Call.ComputerApi.GetComputerEmServers(ComputerEntity.Id);
                gvComServers.DataBind();
            }
            else if (ddlUtil.Text == "Image Servers")
            {
                gvComServers.Visible = true;
                gvComServers.DataSource = Call.ComputerApi.GetComputerImageServers(ComputerEntity.Id);
                gvComServers.DataBind();
            }
            else if (ddlUtil.Text == "Tftp Servers")
            {
                gvComServers.Visible = true;
                gvComServers.DataSource = Call.ComputerApi.GetComputerTftpServers(ComputerEntity.Id);
                gvComServers.DataBind();
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