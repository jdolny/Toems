using System;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin
{
    public partial class AdminMaster : MasterBaseMaster
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