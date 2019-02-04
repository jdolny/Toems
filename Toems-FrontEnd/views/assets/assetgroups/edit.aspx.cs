using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.assets.assetgroups
{
    public partial class edit : BasePages.Assets
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                PopulateForm();
        }

        private void PopulateForm()
        {
            txtName.Text = AssetGroup.Name;
            txtDescription.Text = AssetGroup.Description;
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            AssetGroup.Name = txtName.Text;
            AssetGroup.Description = txtDescription.Text;

            var result = Call.AssetGroupApi.Put(AssetGroup.Id, AssetGroup);
            EndUserMessage = result.Success ? "Successfully Updated Asset Group" : result.ErrorMessage;
        }
    }
}