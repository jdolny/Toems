using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class groupimageacls : BasePages.Users
    {
        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();
            var listImages = (List<ImageWithDate>)gvImages.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listImages = GetSortDirection(e.SortExpression) == "Asc"
                        ? listImages.OrderBy(h => h.Name).ToList()
                        : listImages.OrderByDescending(h => h.Name).ToList();
                    break;
                case "LastUsed":
                    listImages = GetSortDirection(e.SortExpression) == "Asc"
                        ? listImages.OrderBy(h => h.LastUsed).ToList()
                        : listImages.OrderByDescending(h => h.LastUsed).ToList();
                    break;
            }
            gvImages.DataSource = listImages;
            gvImages.DataBind();
            PopulateSizes();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            chkEnableImage.Checked = ToemsUserGroup.EnableImageAcls;

            var filter = new DtoSearchFilterCategories();
            filter.Limit = int.MaxValue;
            var listOfGroups = Call.ImageApi.Search(filter);
            gvImages.DataSource = listOfGroups;
            gvImages.DataBind();

            var managedImages = Call.UserGroupApi.GetManagedImageIds(ToemsUserGroup.Id).ToList();
            foreach (GridViewRow row in gvImages.Rows)
            {
                if (managedImages.Contains(Convert.ToInt32(gvImages.DataKeys[row.RowIndex].Value)))
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (cb != null) cb.Checked = true;
                }
            }


            PopulateSizes();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvImages);
        }

        protected void PopulateSizes()
        {
            foreach (GridViewRow row in gvImages.Rows)
            {
                var lbl = row.FindControl("lblSize") as Label;
                if (lbl != null) lbl.Text = Call.ImageApi.GetImageSizeOnServer(row.Cells[5].Text, "0");
            }
        }
        protected void btnHds_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var row = (GridViewRow)control.Parent.Parent;
            var gvHDs = (GridView)row.FindControl("gvHDs");
            var imageId = ((HiddenField)row.FindControl("HiddenID")).Value;
            var btn = (LinkButton)row.FindControl("btnHDs");

            if (gvHDs.Visible == false)
            {
                var td = row.FindControl("tdHds");
                td.Visible = true;
                gvHDs.Visible = true;
                var schemaRequestOptions = new DtoImageSchemaRequest();

                schemaRequestOptions.image = Call.ImageApi.Get(Convert.ToInt32(imageId));
                schemaRequestOptions.imageProfile = null;
                schemaRequestOptions.schemaType = "deploy";


                gvHDs.DataSource = Call.ImageSchemaApi.GetHardDrives(schemaRequestOptions);
                gvHDs.DataBind();
                btn.Text = "-";
            }
            else
            {
                var td = row.FindControl("tdHds");
                td.Visible = false;
                gvHDs.Visible = false;
                btn.Text = "+";
            }

            foreach (GridViewRow hdrow in gvHDs.Rows)
            {
                var selectedHd = hdrow.RowIndex;
                var lbl = hdrow.FindControl("lblHDSize") as Label;
                if (lbl != null)
                    lbl.Text = Call.ImageApi.GetImageSizeOnServer(row.Cells[5].Text, selectedHd.ToString());
            }
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            ToemsUserGroup.EnableImageAcls = chkEnableImage.Checked;
            var result = Call.UserGroupApi.Put(ToemsUserGroup.Id, ToemsUserGroup);
            if (!result.Success)
            {
                EndUserMessage = "Could Not Set Image Management Status";
                return;
            }

            var memberships = (from GridViewRow row in gvImages.Rows
                               let cb = (CheckBox)row.FindControl("chkSelector")
            where cb != null && cb.Checked
                               select gvImages.DataKeys[row.RowIndex]
             into dataKey
                               where dataKey != null
                               select new EntityUserGroupImages()
                               {
                                   ImageId = Convert.ToInt32(dataKey.Value),
                                   UserGroupId = ToemsUserGroup.Id
                               }).ToList();
            result = Call.UserGroupApi.UpdateImageManagement(memberships,ToemsUserGroup.Id);
            EndUserMessage = result.Success
                ? "Successfully Updated Image Management"
                : "Could Not Update Image Management." + result.ErrorMessage;
        }
    }
}