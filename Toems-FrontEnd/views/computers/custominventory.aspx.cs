using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.computers
{
    public partial class custominventory : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateGrid();
        }

        private void PopulateGrid()
        {
            gvSoftware.DataSource = Call.ComputerApi.GetCustomInventory(ComputerEntity.Id);
            gvSoftware.DataBind();
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gvSoftware_OnSorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var list = (List<DtoCustomComputerInventory>)gvSoftware.DataSource;
            switch (e.SortExpression)
            {
                case "ModuleName":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.ModuleName).ToList()
                        : list.OrderBy(h => h.ModuleName).ToList();
                    break;
                case "Value":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Value).ToList()
                        : list.OrderBy(h => h.Value).ToList();
                    break;


            }

            gvSoftware.DataSource = list;
            gvSoftware.DataBind();
        }
    }
}