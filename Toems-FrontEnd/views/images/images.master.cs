using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.images
{
    public partial class images : BasePages.MasterBaseMaster
    {
        public EntityImage ImageEntity { get; set; }
        private BasePages.Images ImageBasePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ImageBasePage = Page as BasePages.Images;
            ImageEntity = ImageBasePage.ImageEntity;
            if (ImageEntity == null)
            {
                Level2.Visible = false;
                btnDelete.Visible = false;
            }
            else
            {
                Level1.Visible = false;
                btnDelete.Visible = true;
            }

            if (Request.QueryString["sub"] == "profiles")
            {
                btnDelete.Visible = false;
                Level1.Visible = false;
                Level2.Visible = false;
            }
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            var action = (string)Session["action"];
            Session.Remove("action");

            var result = new DtoActionResult();
            var actionLabel = string.Empty;
            switch (action)
            {
                case "delete":
                    result = ImageBasePage.Call.ImageApi.Delete(ImageEntity.Id);
                    actionLabel = "Deleted";
                    break;
             
            }


            if (result.Success)
            {
                PageBaseMaster.EndUserMessage = "Successfully " + actionLabel + " Image: " + ImageEntity.Name;
                if (action.Equals("delete"))
                    Response.Redirect("~/views/images/search.aspx");
            }
            else
                PageBaseMaster.EndUserMessage = result.ErrorMessage;
        }



        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + ImageEntity.Name + "?";
            Session["action"] = "delete";
            DisplayConfirm();

        }





    }
}