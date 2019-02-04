using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.computers
{
    public partial class software : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                PopulateGrid();
        }

        private void PopulateGrid()
        {
            gvSoftware.DataSource = Call.ComputerApi.GetComputerSoftware(ComputerEntity.Id,txtSearch.Text);
            gvSoftware.DataBind();
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }
        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            ExportToCSV(gvSoftware, ComputerEntity.Name + ".csv");
        }

        protected void gvSoftware_OnSorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var list = (List<EntitySoftwareInventory>)gvSoftware.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Name).ToList()
                        : list.OrderBy(h => h.Name).ToList();
                    break;
                case "Version":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Version).ToList()
                        : list.OrderBy(h => h.Version).ToList();
                    break;
              

            }

            gvSoftware.DataSource = list;
            gvSoftware.DataBind();
        }
    }
}