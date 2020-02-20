using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.groups
{
    public partial class imaging : BasePages.Groups
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void ddlComputerImage_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateProfiles();
        }

        private void PopulateProfiles()
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

        private void PopulateForm()
        {
            PopulateImagesDdl(ddlComputerImage);
            ddlComputerImage.SelectedValue = GroupEntity.ImageId.ToString();
            PopulateProfiles();
            ddlImageProfile.SelectedValue = GroupEntity.ImageProfileId.ToString();
            txtPriority.Text = GroupEntity.ImagingPriority.ToString();
            ddlBootFile.Text = GroupEntity.ProxyBootloader;
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            GroupEntity.ImageId = Convert.ToInt32(ddlComputerImage.SelectedValue);
            GroupEntity.ImageProfileId =
                Convert.ToInt32(ddlComputerImage.SelectedValue) == -1
                    ? -1
                    : Convert.ToInt32(ddlImageProfile.SelectedValue);

            var priority = 0;
            if (!int.TryParse(txtPriority.Text, out priority))
            {
                EndUserMessage = "Priority Not Valid";
                return;
            }
            GroupEntity.ImagingPriority = priority;
            GroupEntity.ProxyBootloader = ddlBootFile.Text;

            var result = Call.GroupApi.Put(GroupEntity.Id, GroupEntity);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Group {0}", GroupEntity.Name) : result.ErrorMessage;
        }
    }
}