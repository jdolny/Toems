using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.assets.assettypes
{
    public partial class create : BasePages.Assets
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void buttonAdd_OnClick(object sender, EventArgs e)
        {
            var assetType = new EntityCustomAssetType();
            assetType.Name = txtName.Text;
            assetType.Description = txtDescription.Text;

            var result = Call.CustomAssetTypeApi.Post(assetType);
            if (result.Success)
            {
                EndUserMessage = "Successfully Created Asset Type";
                Response.Redirect("~/views/assets/assettypes/edit.aspx?level=2&assetTypeId=" + result.Id);
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
}