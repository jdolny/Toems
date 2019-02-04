using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.modules.softwaremodules
{
    public partial class categories : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            gvCategories.DataSource = Call.CategoryApi.Get();
            gvCategories.DataBind();

            var moduleCategories = Call.ModuleApi.GetModuleCategories(SoftwareModule.Guid);
            var entityModuleCategories = moduleCategories as EntityModuleCategory[] ?? moduleCategories.ToArray();
            if (entityModuleCategories.Any())
            {
                foreach (GridViewRow row in gvCategories.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    var dataKey = gvCategories.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;

                    foreach (var cat in entityModuleCategories)
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
            var list = new List<EntityModuleCategory>();
            foreach (GridViewRow row in gvCategories.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvCategories.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var modCategory = new EntityModuleCategory();
                modCategory.CategoryId = Convert.ToInt32(dataKey.Value);
                modCategory.ModuleGuid = SoftwareModule.Guid;

                list.Add(modCategory);
            }

            if (list.Count == 0)
            {
                var result = Call.ModuleCategoryApi.Delete(SoftwareModule.Guid);
                if (result != null) EndUserMessage = result.Success ? "Successfully Updated Module." : result.ErrorMessage;
            }
            else
            {
                var z = Call.ModuleCategoryApi.Post(list);
                if (z != null) EndUserMessage = z.Success ? "Successfully Updated Module." : z.ErrorMessage;
            }


        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvCategories);
        }
    }
}