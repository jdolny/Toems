using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.assets.customassets
{
    public partial class create : BasePages.Assets
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateAssetTypes(ddlAssetType);
                ddlAssetType.Items.Remove(ddlAssetType.Items.FindByText("Built-In Any"));
                ddlAssetType.Items.Remove(ddlAssetType.Items.FindByText("Built-In Computers"));
            }
        }

        protected void buttonAdd_OnClick(object sender, EventArgs e)
        {
            var asset = new EntityAsset();
            
            asset.DisplayName = txtName.Text;
            asset.AssetTypeId = Convert.ToInt32(ddlAssetType.SelectedValue);

            var result = Call.AssetApi.Post(asset);
            if (result.Success)
            {
                EndUserMessage = "Successfully Created Asset";
                Response.Redirect("~/views/assets/customassets/edit.aspx?level=2&assetId=" + result.Id);
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
}