using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.assets.assettypes
{
    public partial class assettypes : BasePages.MasterBaseMaster
    {
        public EntityCustomAssetType AssetType { get; set; }
        private BasePages.Assets AssetBasePage { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            AssetBasePage = Page as BasePages.Assets;
            AssetBasePage.RequiresAuthorization(AuthorizationStrings.AssetTypeRead);
            AssetType = AssetBasePage.AssetType;
            if (AssetType == null)
            {
                divLevel3.Visible = false;
                btnDelete.Visible = false;
            }
            else
            {
                divLevel2.Visible = false;
                btnDelete.Visible = true;
            }
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + AssetType.Name + "?";
            DisplayConfirm();
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {

            var result = new DtoActionResult();
            result = AssetBasePage.Call.CustomAssetTypeApi.Delete(AssetType.Id);
 
            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully Deleted Asset Type: " + AssetType.Name;
                    Response.Redirect("~/views/assets/assettypes/search.aspx?level=2");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }
    }
}