using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.admin.toec
{
    public partial class singledeployjob : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlJobType.DataSource = Enum.GetNames(typeof(EnumToecDeployJob.JobType));
                ddlJobType.DataBind();
            }
        }

        protected void buttonDeploy_Click(object sender, EventArgs e)
        {
            var job = new DtoSingleToecDeploy();
            job.ComputerName = txtName.Text;
            job.Domain = txtDomain.Text;
            job.Username = txtUsername.Text;
            job.Password = txtPassword.Text;
            job.JobType = (EnumToecDeployJob.JobType)Enum.Parse(typeof(EnumToecDeployJob.JobType), ddlJobType.SelectedValue);
            var result = Call.ToecDeployJobApi.RunToecDeploySingle(job);
            if (result)
                EndUserMessage = "Successfully Started Deploy Job.  Check The Application Log For Details.";
            else
                EndUserMessage = "Could Not Start Deploy Job";
        }
    }
}