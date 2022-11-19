using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_FrontEnd.views.admin.comservers;

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

                PopulateToecTargetLists(ddlTargetList);
                ddlTargetList.Items.Insert(0, new ListItem("Select A Target List", "-1"));


                PopulateToecTargetLists(ddlExceptionList);
                ddlExceptionList.Items.Insert(0, new ListItem("Select An Exception List", "-1"));

            }
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {

            var toecDeployJob = new EntityToecDeployJob();
            toecDeployJob.Name = txtName.Text;
            toecDeployJob.Username = txtUsername.Text;
            toecDeployJob.PasswordEncrypted = txtPassword.Text;
            toecDeployJob.Domain = txtDomain.Text;
            toecDeployJob.JobType = (EnumToecDeployJob.JobType)Enum.Parse(typeof(EnumToecDeployJob.JobType), ddlJobType.SelectedValue);
            toecDeployJob.RunMode = (EnumToecDeployJob.RunMode)Enum.Parse(typeof(EnumToecDeployJob.RunMode), ddlRunMode.SelectedValue);
            toecDeployJob.TargetListId = Convert.ToInt32(ddlTargetList.SelectedValue);
            toecDeployJob.ExclusionListId = Convert.ToInt32(ddlExceptionList.SelectedValue);
            toecDeployJob.Enabled = chkJobEnabled.Checked;

            var result = Call.ToecDeployJobApi.Post(toecDeployJob);
            if (!result.Success)
            {
                EndUserMessage = "Could Not create " + txtName.Text + " " + result.ErrorMessage;
            }
            else
            {
                EndUserMessage = "Successfully Created " + txtName.Text;

            }
        }
    }
}