using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.global.categories
{
    public partial class categories : MasterBaseMaster
    {
        public EntityCategory Category { get; set; }
        private BasePages.Global GlobalBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            GlobalBasePage = Page as BasePages.Global;
            GlobalBasePage.RequiresAuthorization(AuthorizationStrings.CategoryRead);
            Category = GlobalBasePage.Category;
            if (Category != null)
            {
                Level1.Visible = false;
                Level2.Visible = true;
                btnDelete.Visible = true;

            }
            else
            {
                Level1.Visible = true;
                Level2.Visible = false;
                btnDelete.Visible = false;
            }

        }
        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + Category.Name + "?";
            DisplayConfirm();
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            var result = new DtoActionResult();
            result = GlobalBasePage.Call.CategoryApi.Delete(Category.Id);

            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully Deleted Category: " + Category.Name;
                Response.Redirect("~/views/global/categories/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }
    }
}