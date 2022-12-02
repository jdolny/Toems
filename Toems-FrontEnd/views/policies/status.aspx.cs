using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.policies
{
    public partial class status : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
           
            var listOfPolicies = Call.PolicyApi.GetAllActiveStatus();
            gvPolicies.DataSource = listOfPolicies;
            gvPolicies.DataBind();

        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();

        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<DtoPinnedPolicy>)gvPolicies.DataSource;
            switch (e.SortExpression)
            {
                case "PolicyName":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.PolicyName).ToList()
                        : listModules.OrderBy(h => h.PolicyName).ToList();
                    break;
              

            }

            gvPolicies.DataSource = listModules;
            gvPolicies.DataBind();
        }

    }
}