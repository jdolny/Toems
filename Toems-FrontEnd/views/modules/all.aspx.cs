using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.modules
{
    public partial class all : BasePages.Modules
    {
        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<DtoModule>)gvModules.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "ModuleType":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.ModuleType).ToList()
                        : listModules.OrderBy(h => h.ModuleType).ToList();
                    break;

            }

            gvModules.DataSource = listModules;
            gvModules.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var limit = int.MaxValue;

            var filter = new DtoModuleSearchFilter();
            filter.Limit = limit;
            filter.Searchstring = "";
            filter.IncludePrinter = true;
            filter.IncludeSoftware = true;
            filter.IncludeCommand = true;
            filter.IncludeFileCopy = true;
            filter.IncludeScript = true;
            filter.IncludeWu = true;
            filter.IncludeUnassigned = false;
            filter.IncludeMessage = true;
            filter.IncludeWinPe = true;
            filter.IncludeWinget = true;

            var modules = Call.PolicyApi.GetAllModules(filter);
            gvModules.DataSource = modules;
            gvModules.DataBind();

        }
    }
}