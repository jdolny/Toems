using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.images.profiles
{
    public partial class create : BasePages.Images
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void buttonCreateProfile_OnClick(object sender, EventArgs e)
        {
            var defaultProfile = Call.ImageApi.SeedDefaultProfile(ImageEntity.Id);
            defaultProfile.ImageId = ImageEntity.Id;
            defaultProfile.Name = txtProfileName.Text;
            defaultProfile.Description = txtProfileDesc.Text;
            var result = Call.ImageProfileApi.Post(defaultProfile);
            if (result.Success)
            {
                EndUserMessage = "Successfully Created Image Profile";
                Response.Redirect("~/views/images/profiles/general.aspx?imageid=" + defaultProfile.ImageId +
                                  "&profileid=" +
                                  result.Id + "&sub=profiles");
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
}