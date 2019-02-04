using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.assets.assetgroups
{
    public partial class create : BasePages.Assets
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void buttonAdd_OnClick(object sender, EventArgs e)
        {
            var assetGroup = new EntityAssetGroup();

            assetGroup.Name = txtName.Text;
            assetGroup.Description = txtDescription.Text;

            var result = Call.AssetGroupApi.Post(assetGroup);
            if (result.Success)
            {
                EndUserMessage = "Successfully Created Asset Group";
                Response.Redirect("~/views/assets/assetgroups/edit.aspx?level=2&assetGroupId=" + result.Id);
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
}