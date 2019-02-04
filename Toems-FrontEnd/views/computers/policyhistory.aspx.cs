using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.computers
{
    public partial class policyhistory : BasePages.Computers
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

        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            ExportToCSV(gvHistory, ComputerEntity.Name + ".csv");
        }
        protected void PopulateGrid()
        {

            gvHistory.DataSource = Call.ComputerApi.GetPolicyHistory(ComputerEntity.Id);
            gvHistory.DataBind();
        }

        protected void gvHistory_OnSorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var list = (List<DtoComputerPolicyHistory>)gvHistory.DataSource;
            switch (e.SortExpression)
            {
                case "PolicyName":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.PolicyName).ToList()
                        : list.OrderBy(h => h.PolicyName).ToList();
                    break;
                case "Result":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Result).ToList()
                        : list.OrderBy(h => h.Result).ToList();
                    break;
                case "RunTime":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.RunTime).ToList()
                        : list.OrderBy(h => h.RunTime).ToList();
                    break;
                case "PolicyHash":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.PolicyHash).ToList()
                        : list.OrderBy(h => h.PolicyHash).ToList();
                    break;



            }

            gvHistory.DataSource = list;
            gvHistory.DataBind();
        }
    }
}