using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;

namespace Toems_FrontEnd.views.admin
{
    public partial class about : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            PopulateForm();
        }

        private void PopulateForm()
        {
            lblToemsApi.Text = Call.SettingApi.GetApplicationVersion();
            lblShared.Text = Call.SettingApi.GetSharedLibraryVersion();
            var versions = Call.VersionApi.Get(1);
            lblDatabaseVersion.Text = versions.DatabaseVersion;
            lblExpectedToec.Text = versions.LatestClientVersion;
            lblExpectedToecApi.Text = versions.ExpectedToecApiVersion;
            lblExpectedToemsApi.Text = versions.ExpectedAppVersion;

            var comServers = Call.ClientComServerApi.Get();
            foreach (var comServer in comServers)
            {
                comServerPlaceholder.Controls.Add(new LiteralControl($"<div class=\"size-lbl column\">{comServer.DisplayName}:</div>"));
                var version = Call.ClientComServerApi.GetVersion(comServer.Url);
                comServerPlaceholder.Controls.Add(new LiteralControl($"<div class=\"size-lbl2 column\">{version}"));
                comServerPlaceholder.Controls.Add(new LiteralControl("<br class=\"clear\"/>"));

            }
        }
    }
}