using System;

namespace Toems_FrontEnd.views.policies
{
    public partial class usages : BasePages.Policies
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
           
            gvGroups.Visible = false;
            gvComputers.Visible = false;

            if (ddlUtil.Text == "Computers")
            {
                gvComputers.Visible = true;
                gvComputers.DataSource = Call.PolicyApi.GetPolicyComputers(Policy.Id);
                gvComputers.DataBind();
            }
          
            else
            {
                gvGroups.Visible = true;
                gvGroups.DataSource = Call.PolicyApi.GetPolicyGroups(Policy.Id);
                gvGroups.DataBind();
            }
        }

        protected void ddlUtil_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateForm();
        }
    }
}