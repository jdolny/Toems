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
    public partial class search : Images
    {
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

        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var deleteCount = 0;
            foreach (GridViewRow row in gvImages.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvImages.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                if (Call.ImageApi.Delete(Convert.ToInt32(dataKey.Value)).Success) deleteCount++;
            }
            EndUserMessage = "Successfully Deleted " + deleteCount + " Images";

            PopulateGrid();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DisplayConfirm();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvImages);
        }

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
            var filter = new DtoSearchFilterCategories();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = txtSearch.Text;
            filter.CategoryType = ddlCatType.Text;
            filter.Categories = SelectedCategories();
            var listOfGroups = Call.ImageApi.Search(filter);
            gvImages.DataSource = listOfGroups;
            gvImages.DataBind();

            lblTotal.Text = gvImages.Rows.Count + " Result(s) / " + Call.ImageApi.GetCount() + " Total Image(s)";
            PopulateSizes();
        }

        protected void PopulateSizes()
        {
            foreach (GridViewRow row in gvImages.Rows)
            {
                var lbl = row.FindControl("lblSize") as Label;
                if (lbl != null) lbl.Text = Call.ImageApi.GetImageSizeOnServer(row.Cells[5].Text, "0");
            }
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void ddlCatType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCatType.Text != "Any Category")
            {
                selectCategory.Visible = true;
                CategorySubmit.Visible = true;
            }
            else
            {
                selectCategory.Visible = false;
                CategorySubmit.Visible = false;
            }

            PopulateGrid();
        }
        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }
        protected void CategorySubmit_OnClick(object sender, EventArgs e)
        {

            PopulateGrid();

        }

        private List<string> SelectedCategories()
        {
            var list = new List<string>();
            foreach (ListItem a in selectCategory.Items)
            {

                if (a.Selected)
                    list.Add(a.Value);
            }

            return list;
        }
    }
}