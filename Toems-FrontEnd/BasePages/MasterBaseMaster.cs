using System.Web.UI;
using Toems_ApiCalls;

namespace Toems_FrontEnd.BasePages
{
    public class MasterBaseMaster : MasterPage
    {
        protected virtual void DisplayConfirm()
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "modalscript",
                "$(function() {  var menuTop = document.getElementById('confirmbox'),body = document.body;classie.toggle(menuTop, 'confirm-box-outer-open'); });",
                true);
        }

        public static string GetSetting(string settingName)
        {
            var setting = new APICall().SettingApi.GetSetting(settingName);
            return setting != null ? setting.Value : string.Empty;
        }
    }
}