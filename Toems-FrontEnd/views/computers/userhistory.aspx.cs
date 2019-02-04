using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.computers
{
    public partial class userhistory : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateGrid();
        }

        private void PopulateGrid()
        {
            gvUserHistory.DataSource = Call.ComputerApi.GetUserLogins(ComputerEntity.Id, txtSearch.Text);
            gvUserHistory.DataBind();
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            ExportToCSV(gvUserHistory, ComputerEntity.Name + ".csv");
        }

        protected void gvUserHistory_OnSorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var list = (List<EntityUserLogin>)gvUserHistory.DataSource;
            switch (e.SortExpression)
            {
                case "Username":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.UserName).ToList()
                        : list.OrderBy(h => h.UserName).ToList();
                    break;
                case "LoginDateTime":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.LoginDateTime).ToList()
                        : list.OrderBy(h => h.LoginDateTime).ToList();
                    break;
                case "LogoutDateTime":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.LogoutDateTime).ToList()
                        : list.OrderBy(h => h.LogoutDateTime).ToList();
                    break;


            }

            gvUserHistory.DataSource = list;
            gvUserHistory.DataBind();
        }
    }
}