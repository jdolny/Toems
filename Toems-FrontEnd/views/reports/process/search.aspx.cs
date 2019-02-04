using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.reports.process
{
    public partial class search : BasePages.Report
    {
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvProcesses);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilter();
            filter.Limit = 0;
            filter.SearchText = txtSearch.Text;
            gvProcesses.DataSource = Call.ProcessInventoryApi.Search(filter);
            gvProcesses.DataBind();

            lblTotal.Text = gvProcesses.Rows.Count + " Result(s) / " + Call.ProcessInventoryApi.GetCount() +
                            " Processes";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}