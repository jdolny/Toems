using System;
using Toems_Common;

namespace Toems_FrontEnd.BasePages
{
    public class Report : PageBaseMaster
    {
      

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RequiresAuthorization(AuthorizationStrings.ReportRead);
          
        }

     
    }
}