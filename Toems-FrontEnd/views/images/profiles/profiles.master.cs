using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.images.profiles
{
    public partial class profiles : MasterBaseMaster
    {
        public EntityImage Image { get; set; }
        private Images imageBasePage { get; set; }
        public EntityImageProfile ImageProfile { get; set; }

        public void Page_Load(object sender, EventArgs e)
        {
            imageBasePage = Page as Images;
            ImageProfile = imageBasePage.ImageProfile;
            Image = imageBasePage.ImageEntity;

            if (ImageProfile == null)
            {
                linux_profile.Visible = false;
                winpe_profile.Visible = false;
                return;
            }
            if (Image == null) Response.Redirect("~/", true);
            divProfiles.Visible = false;

            
            if (Image.Environment == "linux" || Image.Environment == "")
            {
                linux_profile.Visible = true;
                winpe_profile.Visible = false;
            }
            else if (Image.Environment == "winpe")
            {
                linux_profile.Visible = false;
                winpe_profile.Visible = true;
            }
        }
    }
}