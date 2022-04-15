using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Toems_ApiCalls;
using Toems_Common.Enum;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.dashboard
{
    public partial class Dashboard : PageBaseMaster
    {
        public string CheckinData { get; set; }

        public string LocalPath { get; set; }
        public string RemotePath { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Only check after login
            if (Request.QueryString["fromlogin"] == "true")
            {
                var userLoginPage = Call.ToemsUserApi.GetUserLoginPage();
                if (userLoginPage.Equals("Active Computers"))
                    Response.Redirect("~/views/computers/search.aspx");
                else if (userLoginPage.Equals("Image Only Computers"))
                    Response.Redirect("~/views/computers/searchimageonly.aspx");
                else if (userLoginPage.Equals("All Computers"))
                    Response.Redirect("~/views/computers/all.aspx");
            }

            if (Request.QueryString["access"] == "denied")
            {
                lblDenied.Text = "You Are Not Authorized For That Action<br><br>";
                var tokenExpired = new APICall().SettingApi.CheckExpiredToken();
                if (tokenExpired)
                {
                    HttpContext.Current.Session.Abandon();
                    FormsAuthentication.SignOut();
                    Response.Redirect("~/?session=expired", true);
                }
            }

          


            PopulateStats();


        }

        protected void PopulateStats()
        {
            try
            {
                var counts = Call.ReportApi.GetCheckinCounts();
                CheckinData = counts.Value;
            }
            catch
            {
                //ignored
            }

            try
            {
                var remote = Call.FilesystemApi.GetSMBFreeSpace();
                if (remote == null)
                {
                    divRemoteStorage.Visible = false;
                }
                else
                {
                    divRemoteStorage.Visible = true;
                    RemotePath = remote.dPPath;
                    lblDPfree.Text = string.Format("Free Space:<b>{0,15:D}</b>",
                        SizeSuffix(Convert.ToInt64(remote.freespace)));
                    lblDpTotal.Text = string.Format("Total Space:<b>{0,15:D}</b>",
                        SizeSuffix(Convert.ToInt64(remote.total)));
                    lblRemotePercent.Text = remote.freePercent + "% Free";
                }
            }
            catch
            {
                //ignored
            }


            try
            {
                var coms = Call.FilesystemApi.GetComFreeSpace();
                if (coms == null)
                {
                    divLocalStorage.Visible = false;
                }
                else
                {
                    foreach (var com in coms)
                    {
                        var divName = new HtmlGenericControl("div");
                        divName.Attributes["class"] = "dash_item";
                        divName.Controls.Add(new LiteralControl("<div class=\"dash_item_content\">"));
                        divName.Controls.Add(new LiteralControl("<span class=\"dash_item_category\">System</span>"));
                        divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                        divName.Controls.Add(new LiteralControl($"<span class=\"dash_item_title\">{com.name} - {com.dPPath}</span></a>"));
                        divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                        var free = string.Format("Free Space:<b>{0,15:D}</b>", SizeSuffix(Convert.ToInt64(com.freespace)));
                        var total = string.Format("Total Space:<b>{0,15:D}</b>", SizeSuffix(Convert.ToInt64(com.total)));
                        var pFree = com.freePercent + "% Free";
                        divName.Controls.Add(new LiteralControl($"{free} {total} {pFree}"));
                        divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                        divName.Controls.Add(new LiteralControl("<br/>"));
                        comServerStorageHolder.Controls.Add(divName);

                    }
                }
            }
            catch
            {
                //ignored
            }


        



            try
            {
                var policyPath = ResolveUrl("~/views/policies/general.aspx");
                var pinnedPolicies = new APICall().ToemsUserApi.GetPinnedPolicies();
                foreach (var pinnedPolicy in pinnedPolicies)
                {
                    var divName = new HtmlGenericControl("div");
                    divName.Attributes["class"] = "dash_item";
                    divName.Controls.Add(new LiteralControl("<span class=\"dash_item_category\">Policy</span>"));
                    divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                    divName.Controls.Add(new LiteralControl($"<a href=\"{policyPath}?policyId={pinnedPolicy.PolicyId}\"><span class=\"dash_item_title\">{pinnedPolicy.PolicyName}</span></a>"));
                    divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                    divName.Controls.Add(new LiteralControl(pinnedPolicy.Description));
                    divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                    divName.Controls.Add(new LiteralControl($"Success: <b>{pinnedPolicy.SuccessCount}</b> Skipped: <b>{pinnedPolicy.SkippedCount}</b> Failed: <b>{pinnedPolicy.FailedCount}</b>"));
                    pinnedPolicyHolder.Controls.Add(divName);

                }
            }
            catch
            {
                //ignored
            }


            try
            {
                var groupPath = ResolveUrl("~/views/groups/general.aspx");
                var pinnedGroups = Call.ToemsUserApi.GetPinnedGroups();
                foreach (var pinnedGroup in pinnedGroups)
                {
                    var divName = new HtmlGenericControl("div");
                    divName.Attributes["class"] = "dash_item";
                    divName.Controls.Add(new LiteralControl("<span class=\"dash_item_category\">Group</span>"));
                    divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                    divName.Controls.Add(new LiteralControl($"<a href=\"{groupPath}?groupId={pinnedGroup.GroupId}\"><span class=\"dash_item_title\">{pinnedGroup.GroupName}</span></a>"));
                    divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                    divName.Controls.Add(new LiteralControl(pinnedGroup.Description));
                    divName.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                    divName.Controls.Add(new LiteralControl($"Computer Count: <b>{pinnedGroup.MemberCount}</b>"));
                    pinnedGroupsHolder.Controls.Add(divName);

                }
            }
            catch
            {
                //ignored
            }
        }

        private string SizeSuffix(long value)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }
            if (value == 0)
            {
                return "0.0 bytes";
            }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
}