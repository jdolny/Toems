using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Dto.imageschemabe;
using Toems_Common.Dto.imageschemafe;

namespace Toems_FrontEnd.views.images.profiles
{
    public partial class deploy : BasePages.Images
    {
        private DropDownList ddlObject;

        protected void btnHd_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var gvRow = (GridViewRow)control.Parent.Parent;
            var gv = (GridView)gvRow.FindControl("gvParts");

            var selectedHd = gvRow.Cells[2].Text;
            ViewState["selectedHD"] = gvRow.RowIndex.ToString();
            ViewState["selectedHDName"] = selectedHd;

            var schemaRequestOptions = new DtoImageSchemaRequest();
            schemaRequestOptions.image = null;
            schemaRequestOptions.imageProfile = ImageProfile;
            schemaRequestOptions.schemaType = "deploy";
            schemaRequestOptions.selectedHd = selectedHd;
            var partitions = Call.ImageSchemaApi.GetPartitions(schemaRequestOptions);
            var btn = (LinkButton)gvRow.FindControl("btnHd");
            if (gv.Visible == false)
            {
                gv.Visible = true;

                var td = gvRow.FindControl("tdParts");
                td.Visible = true;
                gv.DataSource = partitions;
                gv.DataBind();

                btn.Text = "-";
            }
            else
            {
                gv.Visible = false;

                var td = gvRow.FindControl("tdParts");
                td.Visible = false;
                btn.Text = "+";
            }

            foreach (GridViewRow row in gv.Rows)
            {
                if (ddlObject.Text != "Dynamic")
                {
                    foreach (GridViewRow partRow in gv.Rows)
                    {
                        var txtCustomSize = partRow.FindControl("txtCustomSize") as TextBox;
                        if (txtCustomSize != null)
                            txtCustomSize.Enabled = false;

                        var ddlUnit = partRow.FindControl("ddlUnit") as DropDownList;
                        if (ddlUnit != null)
                            ddlUnit.Enabled = false;

                        var chkFixed = partRow.FindControl("chkFixed") as CheckBox;
                        if (chkFixed != null)
                            chkFixed.Enabled = false;
                    }
                }

                if (partitions[row.RowIndex].VolumeGroup == null) continue;
                if (partitions[row.RowIndex].VolumeGroup.Name == null) continue;
                var gvVg = (GridView)row.FindControl("gvVG");
                gvVg.DataSource = new List<Toems_Common.Dto.imageschemafe.DtoVolumeGroup>
                {
                    partitions[row.RowIndex].VolumeGroup
                };
                gvVg.DataBind();

                gvVg.Visible = true;
                var td = row.FindControl("tdVG");
                td.Visible = true;
            }
        }



        protected void btnVG_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var gvRow = (GridViewRow)control.Parent.Parent;
            var gv = (GridView)gvRow.FindControl("gvLVS");

            var selectedHd = (string)ViewState["selectedHD"];

            var btn = (LinkButton)gvRow.FindControl("vgClick");
            if (gv.Visible == false)
            {
                gv.Visible = true;

                var td = gvRow.FindControl("tdLVS");
                td.Visible = true;
                var schemaRequestOptions = new DtoImageSchemaRequest();
                schemaRequestOptions.image = null;
                schemaRequestOptions.imageProfile = ImageProfile;
                schemaRequestOptions.schemaType = "deploy";
                schemaRequestOptions.selectedHd = selectedHd;
                gv.DataSource = Call.ImageSchemaApi.GetLogicalVolumes(schemaRequestOptions);
                gv.DataBind();
                btn.Text = "-";
            }

            else
            {
                gv.Visible = false;
                var td = gvRow.FindControl("tdLVS");
                td.Visible = false;
                btn.Text = "+";
            }

            if (ddlObject.Text != "Dynamic")
            {
                foreach (GridViewRow lv in gv.Rows)
                {
                    var lvTxtCustomSize = lv.FindControl("txtCustomSize") as TextBox;
                    if (lvTxtCustomSize != null)
                        lvTxtCustomSize.Enabled = false;

                    var lvDdlUnit = lv.FindControl("ddlUnit") as DropDownList;
                    if (lvDdlUnit != null)
                        lvDdlUnit.Enabled = false;

                    var lvChkFixed = lv.FindControl("chkFixed") as CheckBox;
                    if (lvChkFixed != null)
                        lvChkFixed.Enabled = false;
                }
            }
        }

        protected void chkForceDynamic_OnCheckedChanged(object sender, EventArgs e)
        {
            if (chkDownForceDynamic.Checked)
            {
                ddlPartitionMethodLin.Enabled = false;
                ddlPartitionMethodLin.Text = "Dynamic";
            }
            else
            {
                ddlPartitionMethodLin.Enabled = true;
            }
        }

        protected void chkModifySchema_OnCheckedChanged(object sender, EventArgs e)
        {
            if (chkModifySchema.Checked)
            {
                imageSchema.Visible = true;
                PopulateHardDrives();
            }
            else
            {
                imageSchema.Visible = false;
            }
        }

        protected void ddlPartitionMethod_OnSelectedIndexChanged(object sender, EventArgs e)
        {

            if (ddlObject.Text == "Standard" || ddlObject.Text == "Standard Core Storage")
            {
                chkModifySchema.Checked = true;
                chkModifySchema.Enabled = false;
                chkDownForceDynamic.Enabled = false;
                chkDownForceDynamic.Checked = true;
            }
            else
            {
                chkModifySchema.Enabled = true;
                chkDownForceDynamic.Enabled = true;
                chkModifySchema.Checked = false;
                chkDownForceDynamic.Checked = false;
            }

            if (ddlObject.Text == "Standard" && ImageEntity.Environment == "linux")
            {
                DivStandardOptions.Visible = true;

            }
            else
            {
                DivStandardOptions.Visible = false;
            }


            DisplayLayout();
        }

        protected void DisplayLayout()
        {
            switch (ddlObject.Text)
            {
                case "Custom Script":
                    customScript.Visible = true;
                    scriptEditorText.Value = ImageProfile.CustomPartitionScript;
                    if (!string.IsNullOrEmpty(ImageProfile.CustomSchema) || chkModifySchema.Checked)
                    {
                        chkModifySchema.Checked = true;
                        imageSchema.Visible = true;
                        PopulateHardDrives();
                    }
                    break;

                default:
                    customScript.Visible = false;

                    if (!string.IsNullOrEmpty(ImageProfile.CustomSchema) || chkModifySchema.Checked)
                    {
                        chkModifySchema.Checked = true;
                        imageSchema.Visible = true;
                        PopulateHardDrives();
                    }

                    break;
            }

            if (ddlObject.Text == "Standard" && ImageEntity.Environment == "linux")
            {
                DivStandardOptions.Visible = true;

            }
        }

        protected void lnkExport_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ImageProfile.CustomSchema))
                EndUserMessage = "You Must Update The Schema First";
            else
            {
                Response.ContentType = "text/plain";
                Response.AppendHeader("Content-Disposition", "attachment; filename=schema.txt");
                Response.Write(ImageProfile.CustomSchema);
                Response.End();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ImageEntity.Environment == "winpe")
                ddlObject = ddlPartitionMethodWin;
            else
                ddlObject = ddlPartitionMethodLin;

            if (IsPostBack) return;
            chkDownNoExpand.Checked = ImageProfile.SkipExpandVolumes;
            chkChangeName.Checked = ImageProfile.ChangeName;
            chkAlignBCD.Checked = ImageProfile.FixBcd;
            chkRunFixBoot.Checked = ImageProfile.FixBootloader;
            chkNvram.Checked = ImageProfile.SkipNvramUpdate;
            chkRandomize.Checked = ImageProfile.RandomizeGuids;
            chkForceEfi.Checked = ImageProfile.ForceStandardEfi;
            chkForceLegacy.Checked = ImageProfile.ForceStandardLegacy;



            if (ImageEntity.Environment == "winpe")
            {
                divExpandVol.Visible = false;
                divBoot.Visible = false;
                ForceDiv.Visible = false;
                DivPartDdlWin.Visible = true;
                ddlPartitionMethodWin.Text = ImageProfile.PartitionMethod;
            }
            else if (ImageEntity.Environment == "linux" || ImageEntity.Environment == "")
            {
                if (ImageEntity.Type == "File")
                    divExpandVol.Visible = false;
                DivPartDdlLin.Visible = true;
                ddlPartitionMethodLin.Text = ImageProfile.PartitionMethod;
                if (chkDownForceDynamic.Checked) ddlPartitionMethodLin.Enabled = false;
                ForceDiv.Visible = ddlPartitionMethodLin.Text == "Dynamic";
            }
            chkDownForceDynamic.Checked = Convert.ToBoolean(ImageProfile.ForceDynamicPartitions);


            DisplayLayout();
        }

        protected void PopulateHardDrives()
        {
            var schemaRequestOptions = new DtoImageSchemaRequest();
            schemaRequestOptions.image = null;
            schemaRequestOptions.imageProfile = ImageProfile;
            schemaRequestOptions.schemaType = "deploy";

            gvHDs.DataSource = Call.ImageSchemaApi.GetHardDrives(schemaRequestOptions);
            gvHDs.DataBind();
        }

        protected string SetCustomSchema()
        {
            var schemaRequestOptions = new DtoImageSchemaRequest();
            schemaRequestOptions.image = null;
            schemaRequestOptions.imageProfile = ImageProfile;
            schemaRequestOptions.schemaType = "deploy";

            var schema = Call.ImageSchemaApi.GetSchema(schemaRequestOptions);

            var rowCounter = 0;
            foreach (GridViewRow row in gvHDs.Rows)
            {
                var box = row.FindControl("chkHDActive") as CheckBox;
                if (box != null)
                    schema.HardDrives[rowCounter].Active = box.Checked;

                var txtDestination = row.FindControl("txtDestination") as TextBox;
                if (txtDestination != null)
                    schema.HardDrives[rowCounter].Destination = txtDestination.Text;

                var gvParts = (GridView)row.FindControl("gvParts");
                var partCounter = 0;
                foreach (GridViewRow partRow in gvParts.Rows)
                {
                    var boxPart = partRow.FindControl("chkPartActive") as CheckBox;
                    if (boxPart != null)
                        schema.HardDrives[rowCounter].Partitions[partCounter].Active = boxPart.Checked;

                    var txtCustomSize = partRow.FindControl("txtCustomSize") as TextBox;
                    if (txtCustomSize != null)
                        schema.HardDrives[rowCounter].Partitions[partCounter].CustomSize = txtCustomSize.Text;

                    var ddlUnit = partRow.FindControl("ddlUnit") as DropDownList;
                    if (ddlUnit != null)
                        schema.HardDrives[rowCounter].Partitions[partCounter].CustomSizeUnit = ddlUnit.Text;

                    var chkFixed = partRow.FindControl("chkFixed") as CheckBox;
                    if (chkFixed != null)
                        schema.HardDrives[rowCounter].Partitions[partCounter].ForceFixedSize = chkFixed.Checked;

                    var gvVg = (GridView)partRow.FindControl("gvVG");

                    foreach (GridViewRow vg in gvVg.Rows)
                    {
                        var gvLvs = (GridView)vg.FindControl("gvLVS");
                        var lvCounter = 0;
                        foreach (GridViewRow lv in gvLvs.Rows)
                        {
                            var lvBoxPart = lv.FindControl("chkPartActive") as CheckBox;
                            if (lvBoxPart != null)
                                schema.HardDrives[rowCounter].Partitions[partCounter].VolumeGroup.LogicalVolumes[
                                    lvCounter]
                                    .Active = lvBoxPart.Checked;

                            var lvTxtCustomSize = lv.FindControl("txtCustomSize") as TextBox;
                            if (lvTxtCustomSize != null)
                                schema.HardDrives[rowCounter].Partitions[partCounter].VolumeGroup.LogicalVolumes[
                                    lvCounter]
                                    .CustomSize = lvTxtCustomSize.Text;

                            var lvDdlUnit = lv.FindControl("ddlUnit") as DropDownList;
                            if (lvDdlUnit != null)
                                schema.HardDrives[rowCounter].Partitions[partCounter].VolumeGroup.LogicalVolumes[
                                    lvCounter]
                                    .CustomSizeUnit = lvDdlUnit.Text;

                            var lvChkFixed = lv.FindControl("chkFixed") as CheckBox;
                            if (lvChkFixed != null)
                                schema.HardDrives[rowCounter].Partitions[partCounter].VolumeGroup.LogicalVolumes[
                                    lvCounter]
                                    .ForceFixedSize = lvChkFixed.Checked;
                            lvCounter++;
                        }
                    }
                    partCounter++;
                }
                rowCounter++;
            }
            return JsonConvert.SerializeObject(schema,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            ImageProfile.ChangeName = chkChangeName.Checked;
            ImageProfile.SkipExpandVolumes = chkDownNoExpand.Checked;
            ImageProfile.FixBcd = chkAlignBCD.Checked;
            ImageProfile.FixBootloader = chkRunFixBoot.Checked;


            ImageProfile.SkipNvramUpdate = chkNvram.Checked;
            ImageProfile.RandomizeGuids = chkRandomize.Checked;
            ImageProfile.ForceStandardLegacy = chkForceLegacy.Checked;
            ImageProfile.ForceStandardEfi = chkForceEfi.Checked;

            if (ImageEntity.Environment == "winpe")
                ImageProfile.PartitionMethod = ddlPartitionMethodWin.Text;
            else
                ImageProfile.PartitionMethod = ddlPartitionMethodLin.Text;
            ImageProfile.ForceDynamicPartitions = chkDownForceDynamic.Checked;


            switch (ddlObject.Text)
            {
                case "Use Original MBR / GPT":
                    ImageProfile.CustomSchema = chkModifySchema.Checked ? SetCustomSchema() : "";
                    break;
                case "Dynamic":

                    ImageProfile.CustomSchema = chkModifySchema.Checked ? SetCustomSchema() : "";
                    break;
                case "Custom Script":
                    var fixedLineEnding = scriptEditorText.Value.Replace("\r\n", "\n");
                    ImageProfile.CustomPartitionScript = fixedLineEnding;
                    ImageProfile.CustomSchema = chkModifySchema.Checked ? SetCustomSchema() : "";
                    break;

                case "Standard":
                    if (ImageEntity.Environment == "winpe" || ImageEntity.Environment == "linux")
                    {
                        ImageProfile.CustomSchema = SetCustomSchema();
                    }
                    else
                    {
                        ImageProfile.CustomSchema = chkModifySchema.Checked ? SetCustomSchema() : "";
                    }
                    break;

                default:
                    ImageProfile.CustomPartitionScript = "";
                    break;
            }

            var isSchemaError = false;
            if (ImageProfile.PartitionMethod == "Standard" && (ImageEntity.Environment == "winpe" || ImageEntity.Environment == "linux"))
            {
                var customSchema = JsonConvert.DeserializeObject<DtoImageSchema>(ImageProfile.CustomSchema);

                foreach (var hd in customSchema.HardDrives)
                {
                    var activePartCounter = hd.Partitions.Count(part => part.Active);
                    if (activePartCounter == 0)
                    {
                        EndUserMessage =
                            "When Using A Standard Partition Layout One Partition With The Operating System Must Be Active.";
                        isSchemaError = true;
                        break;
                    }
                    if (activePartCounter > 1)
                    {
                        EndUserMessage =
                            "When Using A Standard Partition Layout Only One Partition With The Operating System Can Be Active.";
                        isSchemaError = true;
                        break;
                    }
                }
            }

            if (!isSchemaError)
            {
                var result = Call.ImageProfileApi.Put(ImageProfile.Id, ImageProfile);
                EndUserMessage = result.Success ? "Successfully Updated Image Profile" : result.ErrorMessage;
            }
        }
    }
}