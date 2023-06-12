using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin
{
    public partial class winget : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRunWinGet_Click(object sender, EventArgs e)
        {
            Call.SettingApi.RunWinGetImporter();
        }
    }
}