using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_ApiCalls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.BasePages
{
    public class PageBaseMaster : Page
    {
        public APICall Call;
        public EntityToemsUser ToemsCurrentUser;
        public List<string> CurrentUserRights;

        public void ExportToCSV(GridView gridView, string fileName)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Charset = "";
            Response.ContentType = "application/text";

            var sb = new StringBuilder();
            for (int k = 0; k < gridView.Columns.Count; k++)
                sb.Append("\"" + gridView.Columns[k].HeaderText + "\"" + ',');

            sb.Append("\r\n");
            for (int i = 0; i < gridView.Rows.Count; i++)
            {
                for (int k = 0; k < gridView.Columns.Count; k++)
                {
                    if(!string.IsNullOrEmpty(gridView.Rows[i].Cells[k].Text))
                        sb.Append("\"" + gridView.Rows[i].Cells[k].Text + "\"" + ',');
                    else
                    {
                        try
                        {
                            var label = gridView.Rows[i].Cells[k].Controls[1] as Label;
                            if (!string.IsNullOrEmpty(label.Text))
                                sb.Append("\"" + label.Text + "\"" + ',');
                            else
                                sb.Append("\"" + "" + "\"" + ',');
                        }
                        catch
                        {
                            sb.Append("\"" + "" + "\"" + ',');
                            //ignored
                        }
                       
                    }
                   
                }

                sb.Append("\r\n");
            }
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();   
        }

        public void ExportDataSetToCSV(DataSet dataSet, string fileName)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Charset = "";
            Response.ContentType = "application/text";

            var sb = new StringBuilder();
            if (dataSet == null) return;
            if (dataSet.Tables.Count == 0) return;

            DataTable table = dataSet.Tables[0];

            if (table.Rows.Count > 0)
            {
                DataRow dr1 = (DataRow)table.Rows[0];
                int intColumnCount = dr1.Table.Columns.Count;
                int index = 1;

                foreach (DataColumn item in dr1.Table.Columns)
                {
                    sb.Append(String.Format("\"{0}\"", item.ColumnName));
                    if (index < intColumnCount)
                        sb.Append(",");
                    else
                        sb.Append("\r\n");
                    index++;
                }

                foreach (DataRow currentRow in table.Rows)
                {
                    string strRow = string.Empty;
                    for (int y = 0; y <= intColumnCount - 1; y++)
                    {
                        strRow += "\"" + currentRow[y].ToString() + "\"";

                        if (y < intColumnCount - 1 && y >= 0)
                            strRow += ",";
                    }
                    sb.Append(strRow + "\r\n");
                }
            }

            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }



        protected virtual void DisplayConfirm()
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "modalscript",
                "$(function() {  var menuTop = document.getElementById('confirmbox'),body = document.body;classie.toggle(menuTop, 'confirm-box-outer-open'); });",
                true);
        }

     

        public static string EndUserMessage
        {
            get
            {
                if ((string) HttpContext.Current.Session["Message"] != null)
                    return ((string) HttpContext.Current.Session["Message"]).Replace("\\", "\\\\");
                else return null;
            }
            set { HttpContext.Current.Session["Message"] = value; }
        }

        public void ChkAll(GridView gridview)
        {
            var hcb = (CheckBox) gridview.HeaderRow.FindControl("chkSelectAll");

            foreach (GridViewRow row in gridview.Rows)
            {
                var cb = (CheckBox) row.FindControl("chkSelector");
                if (cb != null)
                    cb.Checked = hcb.Checked;
            }
        }

        public void Export(string fileName, string contents)
        {
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AppendHeader("Content-Disposition",
                "attachment; filename=" + fileName);
            HttpContext.Current.Response.Write(contents);
            HttpContext.Current.Response.End();
        }

        public static List<string> GetFeLogs()
        {
            var logPath = HttpContext.Current.Server.MapPath("~") + Path.DirectorySeparatorChar + "private" +
                          Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar;

            var logFiles = Directory.GetFiles(logPath, "*.*");
            var result = new List<string>();
            for (var x = 0; x < logFiles.Length; x++)
                result.Add(Path.GetFileName(logFiles[x]));

            return result;
        }

        public static List<string> GetLogContents(string name, int limit)
        {
            var logPath = HttpContext.Current.Server.MapPath("~") + Path.DirectorySeparatorChar + "private" +
                          Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + name;
            return File.ReadLines(logPath).Reverse().Take(limit).Reverse().ToList();
        }

        public static string GetSetting(string settingName)
        {
            var setting = new APICall().SettingApi.GetSetting(settingName);
            return setting != null ? setting.Value : string.Empty;
        }

        public string GetSortDirection(string sortExpression)
        {
            if (ViewState[sortExpression] == null)
                ViewState[sortExpression] = "Asc";
            else
                ViewState[sortExpression] = ViewState[sortExpression].ToString() == "Desc" ? "Asc" : "Desc";

            return ViewState[sortExpression].ToString();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Call = new APICall();
            var currentUser = Session["ToemsUser"];

            if (currentUser == null)
            {
                HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
                Response.Redirect("~/?session=expired", true);
            }

            ToemsCurrentUser = (EntityToemsUser) currentUser;
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            if (string.IsNullOrEmpty(EndUserMessage)) return;
            const string msgType = "showSuccessToast";
            var page = HttpContext.Current.CurrentHandler as Page;

            if (page != null)
                page.ClientScript.RegisterStartupScript(GetType(), "msgBox",
                    "$(function() { $().toastmessage('" + msgType + "', " + "\"" + EndUserMessage + "\"); });", true);
            HttpContext.Current.Session.Remove("Message");
        }


        public void RequiresAuthorization(string requiredRight)
        {
            if (!Call.AuthorizationApi.IsAuthorized(requiredRight))
                Response.Redirect("~/views/dashboard/dash.aspx?access=denied", true);
        }

        protected void PopulateClusterGroupsDdl(DropDownList ddlClusterGroup)
        {
            ddlClusterGroup.DataSource =
                Call.ComServerClusterApi.Get().Select(d => new { d.Id, d.Name });
            ddlClusterGroup.DataValueField = "Id";
            ddlClusterGroup.DataTextField = "Name";
            ddlClusterGroup.DataBind();
            ddlClusterGroup.Items.Insert(0, new ListItem("Default", "-1"));
        }

        protected void PopulateImpersonationDdl(DropDownList ddlImpersonation)
        {
            ddlImpersonation.DataSource =
                Call.ImpersonationAccountApi.GetForDropDown().Select(d => new { d.Id, d.Username });
            ddlImpersonation.DataValueField = "Id";
            ddlImpersonation.DataTextField = "Username";
            ddlImpersonation.DataBind();
            ddlImpersonation.Items.Insert(0, new ListItem("Default", "-1"));
        }

        protected void PopulateScheduleDdl(DropDownList ddlSchedule)
        {
            ddlSchedule.DataSource =
                Call.ScheduleApi.Get().Select(d => new { d.Id, d.Name });
            ddlSchedule.DataValueField = "Id";
            ddlSchedule.DataTextField = "Name";
            ddlSchedule.DataBind();
            ddlSchedule.Items.Insert(0, new ListItem("Disabled", "-1"));
        }

        protected void PopulateConditions(DropDownList ddlCondition)
        {
            ddlCondition.DataSource =
                Call.ScriptModuleApi.GetConditions().Select(d => new { d.Id, d.Name });
            ddlCondition.DataValueField = "Id";
            ddlCondition.DataTextField = "Name";
            ddlCondition.DataBind();
            ddlCondition.Items.Insert(0, new ListItem("Disabled", "-1"));
        }

        protected void PopulateComServers(DropDownList ddlComServers)
        {
            ddlComServers.DataSource =
                Call.ClientComServerApi.Get().Select(d => new { d.Id, d.DisplayName });
            ddlComServers.DataValueField = "Id";
            ddlComServers.DataTextField = "DisplayName";
            ddlComServers.DataBind();

        }

        protected void PopulateMulticastComServers(DropDownList ddlComServers)
        {
            ddlComServers.DataSource =
                Call.ClientComServerApi.Get().Where(x=> x.IsMulticastServer).Select(d => new { d.Id, d.DisplayName });
            ddlComServers.DataValueField = "Id";
            ddlComServers.DataTextField = "DisplayName";
            ddlComServers.DataBind();

        }

        protected void PopulateComServerUrls(DropDownList ddlComServers)
        {
            ddlComServers.DataSource =
            Call.ClientComServerApi.Get().Select(d => new { d.Id, d.Url });
            ddlComServers.DataValueField = "Id";
            ddlComServers.DataTextField = "Url";
            ddlComServers.DataBind();

        }

        protected void PopulateWinPeModulesDDL(DropDownList ddlModules)
        {
            ddlModules.DataSource =
            Call.WinPeModuleApi.Get().Select(d => new { d.Id, d.Name });
            ddlModules.DataValueField = "Id";
            ddlModules.DataTextField = "Name";
            ddlModules.DataBind();
            ddlModules.Items.Insert(0, new ListItem("Select WinPE Module", "-1"));

        }

        protected void PopulateToecTargetLists(DropDownList ddl)
        {
            ddl.DataSource =
            Call.ToecTargetListApi.Get().Select(d => new { d.Id, d.Name });
            ddl.DataValueField = "Id";
            ddl.DataTextField = "Name";
            ddl.DataBind();

        }

        protected void PopulateDeployJobs(DropDownList ddl)
        {
            ddl.DataSource =
            Call.ToecDeployJobApi.Get().Select(d => new { d.Id, d.Name });
            ddl.DataValueField = "Id";
            ddl.DataTextField = "Name";
            ddl.DataBind();

        }

        protected void PopulateImageProfilesDdl(DropDownList ddlImageProfile, int value)
        {
            ddlImageProfile.DataSource = Call.ImageApi.GetImageProfiles(value).Select(i => new { i.Id, i.Name });
            ddlImageProfile.DataValueField = "Id";
            ddlImageProfile.DataTextField = "Name";
            ddlImageProfile.DataBind();
            ddlImageProfile.Items.Insert(0, new ListItem("Select Profile", "-1"));
        }

        protected void PopulateImagesDdl(DropDownList ddlImages)
        {
            ddlImages.DataSource =
                Call.ImageApi.Get().Select(i => new { i.Id, i.Name }).OrderBy(x => x.Name).ToList();
            ddlImages.DataValueField = "Id";
            ddlImages.DataTextField = "Name";
            ddlImages.DataBind();
            ddlImages.Items.Insert(0, new ListItem("Select Image", "-1"));
        }




    }
}