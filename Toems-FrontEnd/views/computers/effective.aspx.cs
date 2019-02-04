using System;
using System.Web.UI.WebControls;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.computers
{
    public partial class effective : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateTriggers(ddlTrigger);
                ddlTrigger.Items.Remove("StartupOrCheckin");
                ddlTrigger.Items.Insert(0, new ListItem("", ""));
                ddlTrigger.SelectedIndex = 1;

                PopulateComServerUrls(ddlComServer);
                ddlComServer.Items.Insert(0, new ListItem("", ""));
                ddlComServer.SelectedIndex = 1;

                PopulateForm();
            }
        }

        protected void ddlTrigger_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateForm();
        }

        private void PopulateForm()
        {
              scriptEditor.Value = Call.ComputerApi.GetEffectivePolicy(ComputerEntity.Id,
                (EnumPolicy.Trigger) Enum.Parse(typeof (EnumPolicy.Trigger), ddlTrigger.SelectedValue),ddlComServer.SelectedItem.Text);
        }
    }
}