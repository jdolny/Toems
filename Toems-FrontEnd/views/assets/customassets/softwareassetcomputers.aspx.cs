using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.assets.customassets
{
    public partial class softwareassetcomputers : BasePages.Assets
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            gvComputers.DataSource = Call.AssetApi.GetAssetSoftwareComputers(Asset.Id);
            gvComputers.DataBind();
            lblTotal.Text = gvComputers.Rows.Count + " Result(s)";
        }
    }
}