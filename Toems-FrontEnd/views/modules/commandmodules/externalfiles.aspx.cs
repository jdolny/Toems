using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.modules.commandmodules
{
    public partial class externalfiles : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateProgressGrid();
                BindGrid();
            }
            else
            {
                var dt = (DataTable)ViewState["externalDownloads"];
                if (dt.Rows.Count == 0)
                {
                    gvExternalFiles.Rows[0].Cells.Clear();
                }
            }
        }

        protected void BindGrid()
        {
            DataTable dt;
            if (ViewState["externalDownloads"] != null)
            {
                dt = (DataTable)ViewState["externalDownloads"];
                if (dt.Rows.Count > 0)
                {

                    gvExternalFiles.DataSource = dt;
                    gvExternalFiles.DataBind();
                }
            }
            else
            {

                dt = new DataTable();
                dt.Columns.Add("FileName");
                dt.Columns.Add("Url");
                dt.Columns.Add("Hash");
                var dataRow = dt.NewRow();
                dt.Rows.Add(dataRow);
                gvExternalFiles.DataSource = dt;
                gvExternalFiles.DataBind();
                gvExternalFiles.Rows[0].Cells.Clear();

                var emptyTable = new DataTable();
                emptyTable.Columns.Add("FileName");
                emptyTable.Columns.Add("Url");
                emptyTable.Columns.Add("Hash");
                emptyTable.Clear();
                ViewState["externalDownloads"] = emptyTable;

            }

        }


        protected void btnAdd1_OnClick(object sender, EventArgs e)
        {
            var gvRow = (GridViewRow)(sender as Control).Parent.Parent;
            var fileName = ((TextBox)gvRow.FindControl("txtFile")).Text;
            var url = ((TextBox)gvRow.FindControl("txtUrl")).Text;
            var hash = ((TextBox)gvRow.FindControl("txtHash")).Text;
            var dt = (DataTable)ViewState["externalDownloads"];
            var dataRow = dt.NewRow();
            dataRow[0] = fileName;
            dataRow[1] = url;
            dataRow[2] = hash;
            dt.Rows.Add(dataRow);
            ViewState["externalDownloads"] = dt;
            BindGrid();
        }

        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            gvExternalFiles.EditIndex = -1;
            BindGrid();
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var dt = (DataTable)ViewState["externalDownloads"];
            dt.Rows[e.RowIndex].Delete();
            gvExternalFiles.DataSource = dt;
            gvExternalFiles.DataBind();
            if (gvExternalFiles.Rows.Count == 0)
            {
                ViewState["externalDownloads"] = null;
                BindGrid();
            }
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            gvExternalFiles.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var gvRow = gvExternalFiles.Rows[e.RowIndex];
            var dt = (DataTable)ViewState["externalDownloads"];
            dt.Rows[e.RowIndex]["FileName"] = ((TextBox)gvRow.FindControl("txtFile")).Text;
            dt.Rows[e.RowIndex]["Url"] = ((TextBox)gvRow.FindControl("txtUrl")).Text;
            dt.Rows[e.RowIndex]["Hash"] = ((TextBox)gvRow.FindControl("txtHash")).Text;


            ViewState["externalDownloads"] = dt;
            gvExternalFiles.EditIndex = -1;
            BindGrid();
        }

        protected void PopulateProgressGrid()
        {
            var listOfFiles = Call.ExternalDownloadApi.GetForModule(CommandModule.Guid);
            gvFiles.DataSource = listOfFiles;
            gvFiles.DataBind();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvFiles);
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvFiles.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var name = gvRow.Cells[3].Text;
                    var result = Call.ExternalDownloadApi.Delete(Convert.ToInt32(dataKey.Value));
                    if (result.Success) EndUserMessage = "Successfully Removed " + name;
                    else EndUserMessage = result.ErrorMessage;
                    Response.Redirect("~/views/modules/commandmodules/externalfiles.aspx?commandModuleId=" + CommandModule.Id);
                }
            }
        }

        protected void btnReDownload_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.ModuleUploadFiles);
            if (CommandModule.Archived)
            {
                EndUserMessage = "Archived Modules Cannot Download External Files";
                Response.Redirect("~/views/modules/commandmodules/externalfiles.aspx?commandModuleId=" + CommandModule.Id);
                return;
            }

            var activeResult = Call.ModuleApi.IsModuleActive(CommandModule.Guid);
            if (!string.IsNullOrEmpty(activeResult))
            {
                EndUserMessage = activeResult;
                Response.Redirect("~/views/modules/commandmodules/externalfiles.aspx?commandModuleId=" + CommandModule.Id);
                return;
            }
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvFiles.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var name = gvRow.Cells[3].Text;
                    var download = Call.ExternalDownloadApi.Get(Convert.ToInt32(dataKey.Value));
                    var dtoFileDownload = new DtoFileDownload();
                    dtoFileDownload.FileName = download.FileName;
                    dtoFileDownload.ModuleGuid = download.ModuleGuid;
                    dtoFileDownload.Url = download.Url;
                    dtoFileDownload.ExternalDownloadId = download.Id;
                    dtoFileDownload.Sha256 = download.Sha256Hash;
                    Call.ExternalDownloadApi.DownloadFile(dtoFileDownload);
                    EndUserMessage = "Successfully Started Download For " + name;
                    Response.Redirect("~/views/modules/commandmodules/externalfiles.aspx?commandModuleId=" + CommandModule.Id);
                }
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            PopulateProgressGrid();
            UpdatePanel1.Update();
        }

        protected void buttonDownload_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.ModuleUploadFiles);
            if (CommandModule.Archived)
            {
                EndUserMessage = "Archived Modules Cannot Download External Files";
                return;
            }
            var activeResult = Call.ModuleApi.IsModuleActive(CommandModule.Guid);
            if (!string.IsNullOrEmpty(activeResult))
            {
                EndUserMessage = activeResult;
                return;
            }
            var list = new List<DtoFileDownload>();
            var dt = (DataTable)ViewState["externalDownloads"];
            var syncServers = chkSync.Checked;
            if (dt.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvExternalFiles.Rows)
                {
                    var fd = new DtoFileDownload();
                    fd.FileName = ((Label)row.FindControl("lblFile")).Text;
                    if (string.IsNullOrEmpty(fd.FileName))
                    {
                        EndUserMessage = "All Files Must Have A File Name";
                        return;
                    }
                    fd.ModuleGuid = CommandModule.Guid;
                    fd.Url = ((Label)row.FindControl("lblUrl")).Text;
                    if (string.IsNullOrEmpty(fd.Url))
                    {
                        EndUserMessage = "All Files Must Have A URL";
                        return;
                    }
                    fd.Sha256 = ((Label)row.FindControl("lblHash")).Text;
                    fd.SyncWhenDone = syncServers;
                    list.Add(fd);
                }
            }
            else
            {
                EndUserMessage = "No Downloads Were In The List";
                return;
            }
            Call.ExternalDownloadApi.BatchDownload(list);
            EndUserMessage = "Successfully Started Downloads";
            Response.Redirect("~/views/modules/commandmodules/externalfiles.aspx?commandModuleId=" + CommandModule.Id);
        }
    }
}