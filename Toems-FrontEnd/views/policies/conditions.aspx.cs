using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.policies
{
    public partial class conditions : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateForm();
            }
        }



      
        protected void PopulateForm()
        {
            PopulatePolicyComCondition(ddlComCondition);
            PopulateConditions(ddlCondition);
            PopulateConditionFailedAction(ddlConditionFailedAction);
         

            ddlCondition.SelectedValue = Policy.ConditionId.ToString();
            ddlConditionFailedAction.SelectedValue = Policy.ConditionFailedAction.ToString();
          
            ddlComCondition.SelectedValue = Policy.PolicyComCondition.ToString();
         
            BindComServers();


        }

        private void BindComServers()
        {
            var comCondition = (EnumPolicy.PolicyComCondition)Enum.Parse(typeof(EnumPolicy.PolicyComCondition),
                ddlComCondition.SelectedValue);
            if (comCondition == EnumPolicy.PolicyComCondition.Any)
                divComServers.Visible = false;
            else
            {

                divComServers.Visible = true;
                gvServers.DataSource = Call.ClientComServerApi.Get();
                gvServers.DataBind();

                var policyComServers = Call.PolicyApi.GetPolicyComServers(Policy.Id);
                var entityPolicyComServers = policyComServers as EntityPolicyComServer[] ?? policyComServers.ToArray();
                if (entityPolicyComServers.Any())
                {
                    foreach (GridViewRow row in gvServers.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        var dataKey = gvServers.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;

                        foreach (var comServer in entityPolicyComServers)
                        {
                            if (comServer.ComServerId == Convert.ToInt32(dataKey.Value))
                            {
                                cb.Checked = true;
                            }
                        }

                    }
                }
            }

        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (Policy.Archived)
            {
                EndUserMessage = "Archived Policies Cannot Be Modified";
                return;

            }
            Policy.ConditionId = Convert.ToInt32(ddlCondition.SelectedValue);
            Policy.ConditionFailedAction = (EnumCondition.FailedAction)Enum.Parse(typeof(EnumCondition.FailedAction), ddlConditionFailedAction.SelectedValue);
            Policy.PolicyComCondition = (EnumPolicy.PolicyComCondition)Enum.Parse(typeof(EnumPolicy.PolicyComCondition), ddlComCondition.SelectedValue);

            if (Policy.PolicyComCondition == EnumPolicy.PolicyComCondition.Any)
            {
                var r = Call.PolicyComServerApi.Delete(Policy.Id);
                if (!r.Success)
                {
                    EndUserMessage = "Could Not Update Policy Com Servers.";
                    return;
                }
            }
            else
            {
                var list = new List<EntityPolicyComServer>();
                foreach (GridViewRow row in gvServers.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (cb == null || !cb.Checked) continue;
                    var dataKey = gvServers.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;
                    var policyComServer = new EntityPolicyComServer();
                    policyComServer.ComServerId = Convert.ToInt32(dataKey.Value);
                    policyComServer.PolicyId = Policy.Id;

                    list.Add(policyComServer);
                }

                var z = Call.PolicyComServerApi.Post(list);
                if (!z.Success)
                {
                    EndUserMessage = z.ErrorMessage;
                    return;
                }
            }
            var result = Call.PolicyApi.Put(Policy.Id, Policy);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Policy {0}", Policy.Name) : result.ErrorMessage;
        }


        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvServers);
        }

        protected void ddlComCondition_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            BindComServers();
        }
    }
}