using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.policies
{
    public partial class categories : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            gvCategories.DataSource = Call.CategoryApi.Get();
            gvCategories.DataBind();

            var policyCategories = Call.PolicyApi.GetPolicyCategories(Policy.Id);
            var entityPolicyCategories = policyCategories as EntityPolicyCategory[] ?? policyCategories.ToArray();
            if (entityPolicyCategories.Any())
            {
                foreach (GridViewRow row in gvCategories.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    var dataKey = gvCategories.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;

                    foreach (var cat in entityPolicyCategories)
                    {
                        if (cat.CategoryId == Convert.ToInt32(dataKey.Value))
                        {
                            cb.Checked = true;
                        }
                    }

                }
            }

           
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            var list = new List<EntityPolicyCategory>();
            foreach (GridViewRow row in gvCategories.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvCategories.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var policyCategory = new EntityPolicyCategory();
                policyCategory.CategoryId = Convert.ToInt32(dataKey.Value);
                policyCategory.PolicyId = Policy.Id;

                list.Add(policyCategory);
            }

            if (list.Count == 0)
            {
                var result = Call.PolicyCategoryApi.Delete(Policy.Id);
                if(result != null) EndUserMessage = result.Success ? "Successfully Updated Policy." : result.ErrorMessage;
            }
            else
            {
                var z = Call.PolicyCategoryApi.Post(list);
                if (z != null) EndUserMessage = z.Success ? "Successfully Updated Policy." : z.ErrorMessage;
            }
          

        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvCategories);
        }
    }
}