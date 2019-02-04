using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.global.categories
{
    public partial class edit : BasePages.Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        private void PopulateForm()
        {
            txtName.Text = Category.Name;
            txtDescription.Text = Category.Description;
            
        }

        protected void buttonUpdateCategory_OnClick(object sender, EventArgs e)
        {
            Category.Name = txtName.Text;
            Category.Description = txtDescription.Text;
        

            var result = Call.CategoryApi.Put(Category.Id, Category);
            EndUserMessage = result.Success ? "Successfully Updated Category" : result.ErrorMessage;
        }
    }
}