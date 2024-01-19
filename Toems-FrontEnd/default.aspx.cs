using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Toems_ApiCalls;
using Toems_Common.Entity;

namespace Toems_FrontEnd
{
    public partial class Default : Page
    {
        private void ClearSession()
        {
            Session.RemoveAll();
            Session.Abandon();

            FormsAuthentication.SignOut();
            //http://stackoverflow.com/questions/6635349/how-to-delete-cookies-in-asp-net-website
            if (HttpContext.Current != null)
            {
                var cookieCount = HttpContext.Current.Request.Cookies.Count;
                for (var i = 0; i < cookieCount; i++)
                {
                    var cookie = HttpContext.Current.Request.Cookies[i];
                    if (cookie != null)
                    {
                        var cookieName = cookie.Name;
                        var expiredCookie = new HttpCookie(cookieName) { Expires = DateTime.Now.AddDays(-1) };
                        HttpContext.Current.Response.Cookies.Add(expiredCookie); // overwrite it
                    }
                }

                // clear cookies server side
                HttpContext.Current.Request.Cookies.Clear();
            }
        }

        private void CheckDbUpdate()
        {
            HttpCookie baseUrlCookie = Request.Cookies["toemsBaseUrl"];
            if (baseUrlCookie == null)
            {
                var applicationApiUrl = ConfigurationManager.AppSettings["ApplicationApiUrl"];
                if (!applicationApiUrl.EndsWith("/"))
                    applicationApiUrl = applicationApiUrl + "/";
                baseUrlCookie = new HttpCookie("toemsBaseUrl")
                {
                    Value = applicationApiUrl,
                    HttpOnly = true
                };
                Response.Cookies.Add(baseUrlCookie);
                Request.Cookies.Add(baseUrlCookie);
            }
            else
            {
                var applicationApiUrl = ConfigurationManager.AppSettings["ApplicationApiUrl"];
                if (!applicationApiUrl.EndsWith("/"))
                    applicationApiUrl = applicationApiUrl + "/";
                baseUrlCookie.Value = applicationApiUrl;
                Response.Cookies.Add(baseUrlCookie);
                Request.Cookies.Add(baseUrlCookie);
            }

            var versionInfo = new APICall().VersionApi.GetAllVersionInfo();
            if (versionInfo == null)
            {
                lblError.Text = "Could Not Determine Database Version";
                lblError.Visible = true;
                return;
            }
            if (versionInfo.DatabaseVersion != versionInfo.TargetDbVersion)
                Response.Redirect("~/dbupdate.aspx");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var mfaEnabled = string.Empty;
            if (!IsPostBack)
            {
                ClearSession();
                SetBaseUrlCookie();
                if (Request.QueryString["session"] == "expired")
                {
                    SessionExpired.Visible = true;
                    mfaEnabled = new APICall().SettingApi.CheckMfaEnabled();
                    if (mfaEnabled.Equals("1"))
                    {
                        (WebLogin.FindControl("VerifyCode") as TextBox).Visible = true; ;
                    }
                    return;
                }
            }
            else
            {
                SessionExpired.Visible = false;
            }

            
            CheckDbUpdate();

            mfaEnabled = new APICall().SettingApi.CheckMfaEnabled();
            if(mfaEnabled.Equals("1"))
            {
                (WebLogin.FindControl("VerifyCode") as TextBox).Visible = true; ;
            }

          

            if (Request.IsAuthenticated)
            {
                Response.Redirect("~/views/dashboard/dash.aspx");
            }
        }

        protected void WebLogin_Authenticate(object sender, AuthenticateEventArgs e)
        {
            e.Authenticated = false;
           

            //Get token
            var verificationCode = WebLogin.FindControl("VerifyCode") as TextBox;
            var token = new APICall().TokenApi.Get(WebLogin.UserName, WebLogin.Password, verificationCode.Text);
            if (token == null)
            {
                lblError.Text = "Unknown API Error";
                return;
            }

            HttpCookie tokenCookie = Request.Cookies["toemsToken"];
            if (tokenCookie == null)
            {
                tokenCookie = new HttpCookie("toemsToken")
                {
                    Value = token.access_token,
                    HttpOnly = true
                };
                Response.Cookies.Add(tokenCookie);
                Request.Cookies.Add(tokenCookie);
            }
            else
            {
                tokenCookie.Value = token.access_token;
                Response.Cookies.Add(tokenCookie);
                Request.Cookies.Add(tokenCookie);
            }

            if (token.access_token != null)
            {
                //verify token is valid         
                var result = new APICall().ToemsUserApi.GetForLogin(WebLogin.UserName);
                if (result == null)
                {
                    lblError.Text = "Could Not Contact Application API";
                    ClearSession();
                    lblError.Visible = true;
                }
                else if (!result.Success)
                {
                    lblError.Text = result.ErrorMessage == "Forbidden"
                        ? "Token Does Not Match Requested User"
                        : result.ErrorMessage;
                    ClearSession();
                    lblError.Visible = true;
                }
                else if (result.Success)
                {
                    var ToemsUser = JsonConvert.DeserializeObject<EntityToemsUser>(result.ObjectJson);
                    Session["ToemsUser"] = ToemsUser;
                    e.Authenticated = true;

                    Session["ToemsTheme"] = ToemsUser.Theme;

                }
                else
                {
                    ClearSession();
                    lblError.Text = result.ErrorMessage;
                    lblError.Visible = true;
                }
            }
            else
            {
                ClearSession();
                lblError.Text = token.error_description;
                lblError.Visible = true;
            }
        }

        private void SetBaseUrlCookie()
        {
            HttpCookie baseUrlCookie = Request.Cookies["toemsBaseUrl"];
            if (baseUrlCookie == null)
            {
                var applicationApiUrl = ConfigurationManager.AppSettings["ApplicationApiUrl"];
                if (!applicationApiUrl.EndsWith("/"))
                    applicationApiUrl = applicationApiUrl + "/";
                baseUrlCookie = new HttpCookie("toemsBaseUrl")
                {
                    Value = applicationApiUrl,
                    HttpOnly = true
                };
                Response.Cookies.Add(baseUrlCookie);
                Request.Cookies.Add(baseUrlCookie);
            }
            else
            {
                var applicationApiUrl = ConfigurationManager.AppSettings["ApplicationApiUrl"];
                if (!applicationApiUrl.EndsWith("/"))
                    applicationApiUrl = applicationApiUrl + "/";
                baseUrlCookie.Value = applicationApiUrl;
                Response.Cookies.Add(baseUrlCookie);
                Request.Cookies.Add(baseUrlCookie);
            }
        }
       
    }
}