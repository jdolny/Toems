using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.assets.customassets
{
    public partial class currentapplicationrelationship : BasePages.Assets
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            gvSoftware.DataSource = Call.AssetApi.GetSoftware(Asset.Id);
            gvSoftware.DataBind();
        }

        protected void gvSoftware_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            DropDownList ddlMatchType = null;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ddlMatchType = e.Row.FindControl("ddlMatchType") as DropDownList;
                if (ddlMatchType != null)
                {
                    PopulateMatchType(ddlMatchType);
                    ddlMatchType.SelectedValue = ((DtoAssetSoftware)e.Row.DataItem).MatchType.ToString();
                }
            }
        }

        protected void btnRemove_OnClick(object sender, EventArgs e)
        {
            var counter = 0;
            foreach (GridViewRow row in gvSoftware.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvSoftware.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

                var result = Call.SoftwareAssetSoftwareApi.Delete(Convert.ToInt32(row.Cells[3].Text));
                if (result.Success)
                    counter++;
            }


            EndUserMessage = "Removed " + counter + " Asset Applications";

            BindGrid();

        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            var list = new List<EntitySoftwareAssetSoftware>();
            foreach (GridViewRow row in gvSoftware.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvSoftware.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var dropdown = row.FindControl("ddlMatchType") as DropDownList;
                if (dropdown == null) continue;
                var sas = new EntitySoftwareAssetSoftware();
                sas.AssetId = Asset.Id;
                sas.SoftwareInventoryId = Convert.ToInt32(dataKey.Value);
                sas.MatchType = (EnumSoftwareAsset.SoftwareMatchType)Enum.Parse(typeof(EnumSoftwareAsset.SoftwareMatchType), dropdown.SelectedValue);
                list.Add(sas);
            }

            var result = Call.SoftwareAssetSoftwareApi.Post(list);
            if (result != null)
            {
                EndUserMessage = result.Success ? "Successfully Updated Asset Applications" : "Could Not Update Asset Applications";
            }
            else
            {
                EndUserMessage = "Could Not Update Asset Applications";
            }

            BindGrid();

        }
    }
}