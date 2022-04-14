using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;

namespace Toems_FrontEnd.views.computers
{
    public partial class computerviewselector : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var computerView = Call.ToemsUserApi.GetUserComputerView();
            if(string.IsNullOrEmpty(computerView))
                Response.Redirect("~/views/computers/search.aspx");
            else if(computerView.Equals("Active"))
                Response.Redirect("~/views/computers/search.aspx");
            else if (computerView.Equals("Image Only"))
                Response.Redirect("~/views/computers/searchimageonly.aspx");
            else if (computerView.Equals("All"))
                Response.Redirect("~/views/computers/all.aspx");
            else
                Response.Redirect("~/views/computers/search.aspx");
        }
    }
}