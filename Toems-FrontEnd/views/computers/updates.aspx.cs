using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.computers
{
    public partial class updates : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateGrid();
        }

        private void PopulateGrid()
        {
            gvUpdates.DataSource = Call.ComputerApi.GetUpdates(ComputerEntity.Id, txtSearch.Text);
            gvUpdates.DataBind();
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            PopulateGrid();

        }

   

        protected void gvSoftware_OnSorting(object sender, GridViewSortEventArgs e)
        {

            PopulateGrid();

            var list = (List<DtoComputerUpdates>)gvUpdates.DataSource;
            switch (e.SortExpression)
            {
                case "Title":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Title).ToList()
                        : list.OrderBy(h => h.Title).ToList();
                    break;
                case "InstallDate":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.InstallDate).ToList()
                        : list.OrderBy(h => h.InstallDate).ToList();
                    break;
                case "IsInstalled":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.IsInstalled).ToList()
                        : list.OrderBy(h => h.IsInstalled).ToList();
                    break;


            }

            gvUpdates.DataSource = list;
            gvUpdates.DataBind();
        }

        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            ExportToCSV(gvUpdates, ComputerEntity.Name + ".csv");
        }
    }
}