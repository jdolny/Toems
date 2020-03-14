using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin.kernels
{
    public partial class kernels : MasterBaseMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var AdminBasePage = Page as BasePages.Admin;
            AdminBasePage.RequiresAuthorization(AuthorizationStrings.Administrator);
        }
    }
}