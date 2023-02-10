using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using Toems_FrontEnd.BasePages;
using System.Web.UI;
using Toems_ApiCalls;

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
                if (!new APICall().AuthorizationApi.IsAuthorized("moduleRead"))
                    navModules.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("policyRead"))
                    navPolicies.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("groupRead"))
                    navGroups.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("computerRead"))
                    navHosts.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("imageRead"))
                    navImages.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("imageDeployTask"))
                    navTasks.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("assetRead"))
                    navAssets.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("categoryRead"))
                    navGlobal.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("reportRead"))
                    navReport.Visible = false;
                if (!new APICall().AuthorizationApi.IsAuthorized("admin"))
                    navSettings.Visible = false;


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