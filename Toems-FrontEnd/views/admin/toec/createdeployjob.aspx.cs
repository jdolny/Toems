using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.admin.toec
{
    public partial class createdeployjob : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlJobType.DataSource = Enum.GetNames(typeof(EnumToecDeployJob.JobType));
                ddlJobType.DataBind();

                ddlRunMode.DataSource = Enum.GetNames(typeof(EnumToecDeployJob.RunMode));
                ddlRunMode.DataBind();
              
            }
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {

        }
    }
}