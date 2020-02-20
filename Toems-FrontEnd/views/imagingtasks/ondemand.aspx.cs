using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.imagingtasks
{
    public partial class ondemand : BasePages.ImagingTask
    {
        protected void ddlComputerImage_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlComputerImage.Text == "Select Image") return;
            PopulateImageProfilesDdl(ddlImageProfile, Convert.ToInt32(ddlComputerImage.SelectedValue));
            try
            {
                ddlImageProfile.SelectedIndex = 1;
            }
            catch
            {
                //ignore
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            ddlComputerImage.DataSource =
                Call.ImageApi.Get()
                    .Where(
                        x => x.Environment == "linux" || x.Environment == "winpe" || string.IsNullOrEmpty(x.Environment))
                    .Select(i => new { i.Id, i.Name })
                    .OrderBy(x => x.Name)
                    .ToList();
            ddlComputerImage.DataValueField = "Id";
            ddlComputerImage.DataTextField = "Name";
            ddlComputerImage.DataBind();
            ddlComputerImage.Items.Insert(0, new ListItem("Select Image", "-1"));
            PopulateMulticastComServers(ddlComServer);

        }

        protected void btnMulticast_Click(object sender, EventArgs e)
        {
            if (ddlComputerImage.Text == "Select Image") return;


            EndUserMessage = Call.ActiveMulticastSessionApi.StartOnDemandMulticast(Convert.ToInt32(ddlImageProfile.SelectedValue),
                txtClientCount.Text, txtSessionName.Text, Convert.ToInt32(ddlComServer.SelectedValue));
        }
    }
}