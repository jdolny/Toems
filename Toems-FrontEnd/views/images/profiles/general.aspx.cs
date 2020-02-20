using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.images.profiles
{
    public partial class general : BasePages.Images
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtProfileName.Text = ImageProfile.Name;
                txtProfileDesc.Text = ImageProfile.Description;
                txtModelMatch.Text = ImageProfile.ModelMatch;
                ddlMatchCriteria.Text = ImageProfile.ModelMatchType;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var imageProfile = ImageProfile;
            imageProfile.Name = txtProfileName.Text;
            imageProfile.Description = txtProfileDesc.Text;
            imageProfile.ModelMatch = txtModelMatch.Text;
            imageProfile.ModelMatchType = ddlMatchCriteria.Text;
            var result = Call.ImageProfileApi.Put(imageProfile.Id, imageProfile);
            EndUserMessage = result.Success ? "Successfully Updated Image Profile" : result.ErrorMessage;
        }
    }
}