using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using Toems_FrontEnd.BasePages;
using System.Web.UI;

namespace Toems_FrontEnd.views
{
    public partial class SiteMaster : MasterBaseMaster
    {
        

        protected void Page_PreInit(object sender, EventArgs e)
        {
        

        }
        public string CurrentUserName { get; set; }

        public void Page_Init(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
                Response.Redirect("~/", true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var theme = Session["ToemsTheme"];
                if (theme != null)
                {
                    var themeString = theme.ToString();
                    ThemeSheet.Attributes.Add("href", $"~/content/css/themes/{themeString}/style.css");
                }
                
            }
            lblServerId.Text = ConfigurationManager.AppSettings["ServerName"];
            PopulateGrid();
        }


        private void PopulateGrid()
        {
            CurrentUserName = HttpContext.Current.User.Identity.Name;

           
        }

        protected void LogOut_OnClick(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Response.Redirect("~/", true);
        }

       
    }
}