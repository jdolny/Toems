using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.reports.user
{
    public partial class usertracker : BasePages.Report
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BindGrid()
        {
            gvComputers.DataSource = Call.ReportApi.GetUserLogins(txtSearch.Text);
            gvComputers.DataBind();
        }

        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            ExportToCSV(gvComputers, "UserLoginReport.csv");
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void gvComputers_OnSorting(object sender, GridViewSortEventArgs e)
        {
            BindGrid();

            var list = (List<DtoComputerUserLogins>)gvComputers.DataSource;
            switch (e.SortExpression)
            {
                case "ComputerName":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.ComputerName).ToList()
                        : list.OrderBy(h => h.ComputerName).ToList();
                    break;
                case "UserName":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.UserName).ToList()
                        : list.OrderBy(h => h.UserName).ToList();
                    break;
                case "LoginTime":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.LoginTime).ToList()
                        : list.OrderBy(h => h.LoginTime).ToList();
                    break;
                case "LogoutTime":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.LogoutTime).ToList()
                        : list.OrderBy(h => h.LogoutTime).ToList();
                    break;


            }

            gvComputers.DataSource = list;
            gvComputers.DataBind();
        }
    }
}