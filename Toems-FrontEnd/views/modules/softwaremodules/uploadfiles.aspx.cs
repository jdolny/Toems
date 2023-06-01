using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.modules.softwaremodules
{
    public partial class uploadfiles : BasePages.Modules
    {
        protected string token { get; set; }
        protected string baseurl { get; set; }
        protected string moduleGuid { get; set; }
        protected string moduleId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                token = Request.Cookies["toemsToken"].Value;
                baseurl = ConfigurationManager.AppSettings["UploadApiUrl"];
                if (!baseurl.EndsWith("/"))
                    baseurl = baseurl + "/";
                moduleGuid = SoftwareModule.Guid;
                moduleId = SoftwareModule.Id.ToString();
                PopulateGrid();
                if (SoftwareModule.Archived)
                    divSrvUploader.Visible = false;
            }

        }

        protected void UploadButton_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.ModuleUploadFiles);
            var result = Call.SoftwareModuleApi.GenerateArguments(SoftwareModule.Id);
            if (result.Success)
            {
                if(Call.SettingApi.IsStorageRemote())
                    DisplayConfirm(); //prompt for replication
                else
                {
                    EndUserMessage = "Successfully Uploaded File(s)";
                    Response.Redirect("~/views/modules/softwaremodules/uploadfiles.aspx?softwareModuleId=" + SoftwareModule.Id);
                }
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
                Response.Redirect("~/views/modules/softwaremodules/uploadfiles.aspx?softwareModuleId=" + SoftwareModule.Id);
            }
            
           
        }

         protected void ErrorButton_OnClick(object sender, EventArgs e)
         {
             RequiresAuthorization(AuthorizationStrings.ModuleUploadFiles);
            EndUserMessage = uploadErrorMessage.Value;
             Response.Redirect("~/views/modules/softwaremodules/uploadfiles.aspx?softwareModuleId=" + SoftwareModule.Id);
         }

         protected void ButtonConfirm_Click(object sender, EventArgs e)
         {
            Call.HangfireTriggerApi.StartFolderSync();
            EndUserMessage = "Storage Replication Started.";
             Response.Redirect("~/views/modules/softwaremodules/uploadfiles.aspx?softwareModuleId=" + SoftwareModule.Id);
         }

         protected void ButtonCancel_Click(object sender, EventArgs e)
         {
             Response.Redirect("~/views/modules/softwaremodules/uploadfiles.aspx?softwareModuleId=" + SoftwareModule.Id);
         }

        protected void PopulateGrid()
        {
            var listOfFiles = Call.UploadedFileApi.GetModuleFiles(SoftwareModule.Guid);
            gvModules.DataSource = listOfFiles;
            gvModules.DataBind();

            lblTotal.Text = listOfFiles.Count() + " File(s)";
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<EntityUploadedFile>)gvModules.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "Hash":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Hash).ToList()
                        : listModules.OrderBy(h => h.Hash).ToList();
                    break;
                case "DateUploaded":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.DateUploaded).ToList()
                        : listModules.OrderBy(h => h.DateUploaded).ToList();
                    break;

            }

            gvModules.DataSource = listModules;
            gvModules.DataBind();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvModules.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var name = gvRow.Cells[2].Text;
                    var result = Call.UploadedFileApi.Delete(Convert.ToInt32(dataKey.Value));
                    if (result.Success) EndUserMessage = "Successfully Removed " + name;
                    else EndUserMessage = result.ErrorMessage;
                    Response.Redirect("~/views/modules/softwaremodules/uploadfiles.aspx?softwareModuleId=" + SoftwareModule.Id);
                }
            }
        }
    }
}