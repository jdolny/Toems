using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.assets
{
    public partial class assets : BasePages.MasterBaseMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var level = Request.QueryString["level"];
            if (level == null) return;
            if (level == "2")
                Level1.Visible = false;
        }

     
    }
}