using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;
using HtmlGenericControl = System.Web.UI.HtmlControls.HtmlGenericControl;

namespace Toems_FrontEnd.views.computers
{
    public partial class customattributes : BasePages.Computers
    {
        private List<TextBox> _listTextBoxes;

        protected void Page_Load(object sender, EventArgs e)
        {
            var customAttributes = Call.CustomAttributeApi.GetForBuiltInComputers();
            if (customAttributes == null) return;
            if (customAttributes.Count == 0) return;

            _listTextBoxes = new List<TextBox>();
            var computerAttributes = Call.ComputerApi.GetCustomAttributes(ComputerEntity.Id).ToList();

            foreach (var ca in customAttributes)
            {
                var divName = new HtmlGenericControl("div");
                divName.Attributes["class"] = "size-lbl column";
                divName.InnerHtml = ca.Name;

                var txtbox = new TextBox();
                if(ca.TextMode == EnumCustomAttribute.TextMode.MultiLine)           
                    txtbox.TextMode = TextBoxMode.MultiLine;
                txtbox.Attributes["class"] = "textbox";
                txtbox.ID = "txtCA_" + ca.Id;
                var value = computerAttributes.FirstOrDefault(x =>
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

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.ComputerUpdate);
            if (_listTextBoxes == null) return;
            if (_listTextBoxes.Count == 0) return;

            var computerAttributes = new List<EntityCustomComputerAttribute>();
            foreach (TextBox tBox in _listTextBoxes)
            {

                var computerAttribute = new EntityCustomComputerAttribute();
                computerAttribute.ComputerId = ComputerEntity.Id;
                computerAttribute.CustomAttributeId = Convert.ToInt32(tBox.ID.Split('_')[1]);
                computerAttribute.Value = tBox.Text;
                computerAttributes.Add(computerAttribute);

            }

            var result = Call.CustomComputerAttributeApi.Post(computerAttributes);
            if (result.Success)
                EndUserMessage = "Successfully Updated Custom Attributes";
            else
                EndUserMessage = "Could Not Update Custom Attributes";
        }
    }
}