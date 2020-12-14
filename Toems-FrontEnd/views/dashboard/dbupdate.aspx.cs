using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_ApiCalls;

namespace Toems_FrontEnd.views.dashboard
{
    public partial class dbupdate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var version = new APICall().VersionApi.GetAllVersionInfo();
                if (version != null)
                {
                    if (version.DatabaseVersion == version.TargetDbVersion)
                    {
                        lblVersion.Text = "Database Is Up To Date";
                    }
                    else
                    {
                        lblVersion.Text = "Database Update Required";
                    }

                    lblVersion.Text += "<br> Current Version: " + version.DatabaseVersion +
                                       "<br> Required Version: " + version.TargetDbVersion;
                }
                else
                {
                    lblVersion.Text = "Error:  Could Not Determine Current Database Version";
                }
            }

        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {

            var result = new APICall().VersionApi.UpdateDatabase();
            if (result.Success)
            {
                lblResult.Text = "Success";
                Response.Redirect("~/views/dashboard/dash.aspx");
            }
            else
            {
                lblResult.Text = result.ErrorMessage;
            }
        }
    }
}