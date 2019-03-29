using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.computers
{
    public partial class certificates : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateGrid();
        }

        private void PopulateGrid()
        {
            gvCertificates.DataSource = Call.ComputerApi.GetComputerCertificates(ComputerEntity.Id, txtSearch.Text);
            gvCertificates.DataBind();
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }
        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            ExportToCSV(gvCertificates, ComputerEntity.Name + ".csv");
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

            gvCertificates.DataSource = list;
            gvCertificates.DataBind();
        }
    }
}