using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_FrontEnd.views.computers;

namespace Toems_FrontEnd.views.admin.toec
{
    public partial class editdeployjob : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
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

            txtName.Text = ToecDeployJob.Name;
            txtUsername.Text = ToecDeployJob.Username;
            txtDomain.Text = ToecDeployJob.Domain;
            ddlJobType.SelectedValue = ToecDeployJob.JobType.ToString();
            ddlRunMode.SelectedValue = ToecDeployJob.RunMode.ToString();
            ddlTargetList.SelectedValue = ToecDeployJob.TargetListId.ToString();
            ddlExceptionList.SelectedValue = ToecDeployJob.ExclusionListId.ToString();
            chkJobEnabled.Checked = ToecDeployJob.Enabled;


        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            ToecDeployJob.Name = txtName.Text;
            ToecDeployJob.Username = txtUsername.Text;
            if (!string.IsNullOrEmpty(txtPassword.Text))
                ToecDeployJob.PasswordEncrypted = txtPassword.Text;
            else
                ToecDeployJob.PasswordEncrypted = null;
            ToecDeployJob.Domain = txtDomain.Text;
            ToecDeployJob.JobType = (EnumToecDeployJob.JobType)Enum.Parse(typeof(EnumToecDeployJob.JobType), ddlJobType.SelectedValue);
            ToecDeployJob.RunMode = (EnumToecDeployJob.RunMode)Enum.Parse(typeof(EnumToecDeployJob.RunMode), ddlRunMode.SelectedValue);
            ToecDeployJob.TargetListId = Convert.ToInt32(ddlTargetList.SelectedValue);
            ToecDeployJob.ExclusionListId = Convert.ToInt32(ddlExceptionList.SelectedValue);
            ToecDeployJob.Enabled = chkJobEnabled.Checked;

            var result = Call.ToecDeployJobApi.Put(ToecDeployJob.Id, ToecDeployJob);
            EndUserMessage = !result.Success ? result.ErrorMessage : "Successfully Updated Deploy Job";
        }
    }
}