using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.policies
{
    public partial class clienthistory : BasePages.Policies
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
            var filter = new DtoSearchFilter();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = txtSearch.Text;

            gvHistory.DataSource = Call.PolicyApi.GetHistoryWithComputer(Policy.Id,filter);
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

        protected void gvHistory_OnSorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var list = (List<EntityPolicyHistory>)gvHistory.DataSource;
            switch (e.SortExpression)
            {
                case "ComputerName":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.ComputerName).ToList()
                        : list.OrderBy(h => h.ComputerName).ToList();
                    break;
                case "LastRunTime":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.LastRunTime).ToList()
                        : list.OrderBy(h => h.LastRunTime).ToList();
                    break;
                case "Hash":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Hash).ToList()
                        : list.OrderBy(h => h.Hash).ToList();
                    break;
                case "Result":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Result).ToList()
                        : list.OrderBy(h => h.Result).ToList();
                    break;

            }

            gvHistory.DataSource = list;
            gvHistory.DataBind();
        }
    }
}