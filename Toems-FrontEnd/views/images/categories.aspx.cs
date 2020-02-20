using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.images
{
    public partial class categories : BasePages.Images
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

            var imageCategories = Call.ImageApi.GetImageCategories(ImageEntity.Id);
            var entityImageCategories = imageCategories as EntityImageCategory[] ?? imageCategories.ToArray();
            if (entityImageCategories.Any())
            {
                foreach (GridViewRow row in gvCategories.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    var dataKey = gvCategories.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;

                    foreach (var cat in entityImageCategories)
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
            var list = new List<EntityImageCategory>();
            foreach (GridViewRow row in gvCategories.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvCategories.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var imageCategory = new EntityImageCategory();
                imageCategory.CategoryId = Convert.ToInt32(dataKey.Value);
                imageCategory.ImageId = ImageEntity.Id;

                list.Add(imageCategory);
            }

            if (list.Count == 0)
            {
                var result = Call.ImageCategoryApi.Delete(ImageEntity.Id);
                if (result != null) EndUserMessage = result.Success ? "Successfully Updated Image." : result.ErrorMessage;
            }
            else
            {
                var z = Call.ImageCategoryApi.Post(list);
                if (z != null) EndUserMessage = z.Success ? "Successfully Update Image." : z.ErrorMessage;
            }


        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvCategories);
        }
    }
}