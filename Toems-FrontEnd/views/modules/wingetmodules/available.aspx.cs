using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.modules.wingetmodules
{
    public partial class available : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateGrid();
            }
        }

        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }


        protected void PopulateGrid()
        {
            var filter = new DtoWingetSearchFilter();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.Searchstring = txtSearch.Text;
            filter.IncludePackageIdentifier = chkIdentifier.Checked;
            filter.IncludePackageName = chkName.Checked;
            filter.IncludePublisher = chkPublisher.Checked;
            filter.IncludeTags = chkTags.Checked;
            filter.IncludeMoniker = chkMoniker.Checked;
            filter.ExactMatch = chkExact.Checked;
            filter.LatestVersionOnly = chkLatest.Checked;

            var listOfManifests = Call.WingetModuleApi.SearchManifests(filter);
            gvManifests.DataSource = listOfManifests;
            gvManifests.DataBind();

        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<EntityWingetLocaleManifest>)gvManifests.DataSource;
            switch (e.SortExpression)
            {
                case "PackageIdentifier":
                    listModules = GetSortDirection(e.SortExpression) == "PackageIdentifier"
                        ? listModules.OrderByDescending(h => h.PackageIdentifier).ToList()
                        : listModules.OrderBy(h => h.PackageIdentifier).ToList();
                    break;

            }

            gvManifests.DataSource = listModules;
            gvManifests.DataBind();
        }


        protected void chkFilter_OnCheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }


        protected void details_Click(object sender, EventArgs e)
        {
            LinkButton lb = sender as LinkButton;
            var manifest = Call.WingetModuleApi.GetLocaleManifest(Convert.ToInt32(lb.CommandArgument));
            txtPackageName.Text = manifest.PackageName;
            txtPackageIdentifier.Text = manifest.PackageIdentifier;
            txtPackageVersion.Text = manifest.PackageVersion;
            txtPackageUrl.Text = manifest.PackageUrl;
            txtPackageUrl.NavigateUrl = manifest.PackageUrl;
            txtPublisher.Text = manifest.Publisher;
            txtPublisherUrl.NavigateUrl = manifest.PublisherUrl;
            txtPublisherUrl.Text = manifest.PublisherUrl;
            txtLicense.Text = manifest.License;
            txtDescription.Text = manifest.ShortDescription;
            txtTags.Text = manifest.Tags;
            txtMoniker.Text = manifest.Moniker;

            divModal.Visible = true;
        }

        protected void CloseModal(object sender, EventArgs e)
        {
            divModal.Visible = false;
        }


    }
}