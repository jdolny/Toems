using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.reports
{
    public partial class software : BasePages.Report
    {
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvSoftware);
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
            gvSoftware.DataSource = Call.SoftwareInventoryApi.Search(filter);
            gvSoftware.DataBind();

            lblTotal.Text = gvSoftware.Rows.Count + " Result(s) / " + Call.SoftwareInventoryApi.GetCount() +
                            " Software Application(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void btnExport_OnClick(object sender, EventArgs e)
        {
           ExportToCSV(gvSoftware,"software.csv");
        }
    }
}