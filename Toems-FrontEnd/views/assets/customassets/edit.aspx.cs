using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.assets.customassets
{
    public partial class edit : BasePages.Assets
    {
        private List<TextBox> _listTextBoxes;

        protected void Page_Load(object sender, EventArgs e)
        {
            _listTextBoxes = new List<TextBox>();
            if (!IsPostBack)
            {
                PopulateAssetTypes(ddlAssetType);
                ddlAssetType.Items.Remove(ddlAssetType.Items.FindByText("Built-In Any"));
                ddlAssetType.Items.Remove(ddlAssetType.Items.FindByText("Built-In Computers"));
                PopulateForm();
            }
            PopulateAttributes();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            Asset.DisplayName = txtName.Text;
            var result = Call.AssetApi.Put(Asset.Id, Asset);

            if (!result.Success)
            {
                EndUserMessage = result.ErrorMessage;
                return;
            }

            if (_listTextBoxes == null) return;
            if (_listTextBoxes.Count == 0) return;

            var assetAttributes = new List<EntityAssetAttribute>();
            foreach (TextBox tBox in _listTextBoxes)
            {

                var assetAttribute = new EntityAssetAttribute();
                assetAttribute.AssetId = Asset.Id;
                assetAttribute.CustomAttributeId = Convert.ToInt32(tBox.ID.Split('_')[1]);
                assetAttribute.Value = tBox.Text;
                assetAttributes.Add(assetAttribute);

            }

            result = Call.AssetAttributeApi.Post(assetAttributes);
            if (result.Success)
                EndUserMessage = "Successfully Updated Asset Attributes";
            else
                EndUserMessage = "Could Not Update Asset Attributes";

          
        }

        private void PopulateForm()
        {
            txtName.Text = Asset.DisplayName;
            ddlAssetType.SelectedValue = Asset.AssetTypeId.ToString();

         
        }

        private void PopulateAttributes()
        {
            var customAttributes = Call.CustomAttributeApi.GetForAssetType(Asset.AssetTypeId);
            if (customAttributes == null) return;
            if (customAttributes.Count == 0) return;

            _listTextBoxes = new List<TextBox>();
            var assetAttributes = Call.AssetApi.GetAttributes(Asset.Id).ToList();

            foreach (var ca in customAttributes)
            {
                var divName = new HtmlGenericControl("div");
                divName.Attributes["class"] = "size-lbl column";
                divName.InnerHtml = ca.Name;

                var txtbox = new TextBox();
                if (ca.TextMode == EnumCustomAttribute.TextMode.MultiLine)
                {
                    txtbox.TextMode = TextBoxMode.MultiLine;
                    txtbox.Attributes["class"] = "descbox";
                }
                else
                {
                    txtbox.Attributes["class"] = "textbox";
                }
                
                txtbox.ID = "txtCA_" + ca.Id;
                var value = assetAttributes.FirstOrDefault(x =>
                    x.CustomAttributeId == ca.Id && !string.IsNullOrEmpty(x.Value));
                if (value != null)
                    txtbox.Text = value.Value;

                _listTextBoxes.Add(txtbox);
                var divLabel = new HtmlGenericControl("div");
                divLabel.Attributes["class"] = "size-lbl2 column";
                divLabel.Controls.Add(txtbox);

                placeholder.Controls.Add(divName);
                placeholder.Controls.Add(divLabel);
                placeholder.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                placeholder.Controls.Add(new LiteralControl("<br />"));
            }
        }
    }
}