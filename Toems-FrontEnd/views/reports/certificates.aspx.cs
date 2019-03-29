using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.reports
{
    public partial class certificates : BasePages.Report
    {
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvCertificates);
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
            gvCertificates.DataSource = Call.CertificateInventoryApi.Search(filter);
            gvCertificates.DataBind();

            lblTotal.Text = gvCertificates.Rows.Count + " Result(s) / " + Call.CertificateInventoryApi.GetCount() +
                            " Certificate(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            ExportToCSV(gvCertificates, "certificates.csv");
        }

        protected void gvSoftware_OnSorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var list = (List<EntityCertificateInventory>)gvCertificates.DataSource;
            switch (e.SortExpression)
            {
                case "Store":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Store).ToList()
                        : list.OrderBy(h => h.Store).ToList();
                    break;
                case "Subject":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Subject).ToList()
                        : list.OrderBy(h => h.Subject).ToList();
                    break;
                case "FriendlyName":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.FriendlyName).ToList()
                        : list.OrderBy(h => h.FriendlyName).ToList();
                    break;
                case "Thumbprint":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Thumbprint).ToList()
                        : list.OrderBy(h => h.Thumbprint).ToList();
                    break;
                case "Serial":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Serial).ToList()
                        : list.OrderBy(h => h.Serial).ToList();
                    break;
                case "Issuer":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.Issuer).ToList()
                        : list.OrderBy(h => h.Issuer).ToList();
                    break;
                case "NotBefore":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.NotBefore).ToList()
                        : list.OrderBy(h => h.NotBefore).ToList();
                    break;
                case "NotAfter":
                    list = GetSortDirection(e.SortExpression) == "Desc"
                        ? list.OrderByDescending(h => h.NotAfter).ToList()
                        : list.OrderBy(h => h.NotAfter).ToList();
                    break;



            }
        }
    }
}