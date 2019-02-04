using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.global.categories
{
    public partial class create : BasePages.Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void buttonAddCategory_OnClick(object sender, EventArgs e)
        {
            var category = new EntityCategory();
            category.Name = txtName.Text;
            category.Description = txtDescription.Text;
           
            var result = Call.CategoryApi.Post(category);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Category";
                Response.Redirect("~/views/global/categories/edit.aspx?categoryId=" + result.Id);
            }
        }
    }
}