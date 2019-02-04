using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.assets.customassets
{
    public partial class customassets : MasterBaseMaster
    {
        public EntityAsset Asset { get; set; }
        private BasePages.Assets AssetBasePage { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            AssetBasePage = Page as BasePages.Assets;
            AssetBasePage.RequiresAuthorization(AuthorizationStrings.AssetRead);
            Asset = AssetBasePage.Asset;
            if (Asset == null)
            {
                divLevel3.Visible = false;
                btnDelete.Visible = false;
                btnArchive.Visible = false;
            }
            else
            {
                var asset = new APICall().AssetApi.Get(Convert.ToInt32(Asset.Id));
                if (asset != null)
                {
                    if (asset.AssetTypeId == -2) //built-in software
                        softwarelink.Visible = true;
                }
                divLevel2.Visible = false;
                btnDelete.Visible = true;
                btnArchive.Visible = true;
            }
        }

         protected void btnArchive_OnClick(object sender, EventArgs e)
         {
             lblTitle.Text = "Archive " + Asset.DisplayName + "?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete " + Asset.DisplayName + "?";
            Session["action"] = "delete";
            DisplayConfirm();
        }

        protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            var result = new DtoActionResult();
            var action = (string)Session["action"];
            Session.Remove("action");
            var count = 0;
            switch (action)
            {
                case "delete":
                    result = AssetBasePage.Call.AssetApi.Delete(Asset.Id);

                    if (result.Success)
                    {
                        PageBaseMaster.EndUserMessage = "Successfully Deleted Asset: " + Asset.DisplayName;
                        Response.Redirect("~/views/assets/customassets/search.aspx?level=2");
                    }
                    else
                        PageBaseMaster.EndUserMessage = result.ErrorMessage;
                    break;
                case "archive":
                    result = AssetBasePage.Call.AssetApi.Archive(Asset.Id);

                    if (result.Success)
                    {
                        PageBaseMaster.EndUserMessage = "Successfully Archived Asset: " + Asset.DisplayName;
                        Response.Redirect("~/views/assets/customassets/search.aspx?level=2");
                    }
                    else
                        PageBaseMaster.EndUserMessage = result.ErrorMessage;
                    break;
            }
           

        }


    }
}