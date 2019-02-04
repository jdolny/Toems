using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;

namespace Toems_FrontEnd.views.assets.assettypes
{
    public partial class edit : BasePages.Assets
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {

            AssetType.Name = txtName.Text;
            AssetType.Description = txtDescription.Text;

            var result = Call.CustomAssetTypeApi.Put(AssetType.Id, AssetType);
            EndUserMessage = result.Success ? "Successfully Updated Asset Type" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            txtName.Text = AssetType.Name;
            txtDescription.Text = AssetType.Description;
        }
    }
}