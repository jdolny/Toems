using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.dashboard
{
    public partial class keepalive : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Call.SettingApi.CheckExpiredToken();

        }
    }
}