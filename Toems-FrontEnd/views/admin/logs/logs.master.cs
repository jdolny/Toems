using System;
using Toems_Common;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin.logs
{
    public partial class logs : MasterBaseMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var AdminBasePage = Page as BasePages.Admin;
            AdminBasePage.RequiresAuthorization(AuthorizationStrings.Administrator);
        }
    }
}