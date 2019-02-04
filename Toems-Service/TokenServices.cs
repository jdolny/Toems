using System.Web;
using Toems_ApiCalls;

namespace Toems_Service
{
    public class TokenServices
    {
        public string GetToken(string username, string password, string baseUrl)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("toemsBaseUrl")
            {
                Value = baseUrl,
                HttpOnly = true
            });
            var result = new APICall().TokenApi.Get(username, password);
            return !string.IsNullOrEmpty(result.error_description) ? result.error_description : "bearer " + result.access_token;
        }

    
    }
}