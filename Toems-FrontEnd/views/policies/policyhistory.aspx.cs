using System;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.policies
{
    public partial class policyhistory : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateGrid();
            }
        }

        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void PopulateGrid()
        {

            gvHistory.DataSource = Call.PolicyApi.GetPolicyHashHistory(Policy.Id);
            gvHistory.DataBind();
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();


        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvHistory);
        }
    }
}