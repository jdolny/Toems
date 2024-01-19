using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.policies
{
    public partial class create : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            var policy = new EntityPolicy();
            policy.Name = txtName.Text;
            policy.Description = txtDescription.Text;
            policy.StartDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            var result = Call.PolicyApi.Post(policy);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                

                EndUserMessage = "Successfully Created Policy";
                Response.Redirect("~/views/policies/general.aspx?policyId=" + result.Id);
            }
        }
    }
}