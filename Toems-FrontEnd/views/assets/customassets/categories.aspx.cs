using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.assets.customassets
{
    public partial class categories : BasePages.Assets
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

            var assetCategories = Call.AssetApi.GetAssetCategories(Asset.Id);
            var entityAssetCategories = assetCategories as EntityAssetCategory[] ?? assetCategories.ToArray();
            if (entityAssetCategories.Any())
            {
                foreach (GridViewRow row in gvCategories.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    var dataKey = gvCategories.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;

                    foreach (var cat in entityAssetCategories)
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
            var list = new List<EntityAssetCategory>();
            foreach (GridViewRow row in gvCategories.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvCategories.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var assetCategory = new EntityAssetCategory();
                assetCategory.CategoryId = Convert.ToInt32(dataKey.Value);
                assetCategory.AssetId = Asset.Id;

                list.Add(assetCategory);
            }

            if (list.Count == 0)
            {
                var result = Call.AssetCategoryApi.Delete(Asset.Id);
                if (result != null) EndUserMessage = result.Success ? "Successfully Updated Asset." : result.ErrorMessage;
            }
            else
            {
                var z = Call.AssetCategoryApi.Post(list);
                if (z != null) EndUserMessage = z.Success ? "Successfully Updated Asset." : z.ErrorMessage;
            }


        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvCategories);
        }
    }
}