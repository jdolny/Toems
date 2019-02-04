using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.groups
{
    public partial class categories : BasePages.Groups
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

            var groupCategories = Call.GroupApi.GetGroupCategories(GroupEntity.Id);
            var entityGroupCategories = groupCategories as EntityGroupCategory[] ?? groupCategories.ToArray();
            if (entityGroupCategories.Any())
            {
                foreach (GridViewRow row in gvCategories.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    var dataKey = gvCategories.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;

                    foreach (var cat in entityGroupCategories)
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
            var list = new List<EntityGroupCategory>();
            foreach (GridViewRow row in gvCategories.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvCategories.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var groupCategory = new EntityGroupCategory();
                groupCategory.CategoryId = Convert.ToInt32(dataKey.Value);
                groupCategory.GroupId = GroupEntity.Id;

                list.Add(groupCategory);
            }

            if (list.Count == 0)
            {
                var result = Call.GroupCategoryApi.Delete(GroupEntity.Id);
                if (result != null) EndUserMessage = result.Success ? "Successfully Updated Group." : result.ErrorMessage;
            }
            else
            {
                var z = Call.GroupCategoryApi.Post(list);
                if (z != null) EndUserMessage = z.Success ? "Successfully Update Group." : z.ErrorMessage;
            }


        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvCategories);
        }
    }
}