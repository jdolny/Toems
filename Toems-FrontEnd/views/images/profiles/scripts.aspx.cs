using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.images.profiles
{
    public partial class scripts : BasePages.Images
    {
        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();
            var listScripts = (List<EntityScriptModule>)gvScripts.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listScripts = GetSortDirection(e.SortExpression) == "Asc"
                        ? listScripts.OrderBy(s => s.Name).ToList()
                        : listScripts.OrderByDescending(s => s.Name).ToList();
                    break;
            }

            gvScripts.DataSource = listScripts;
            gvScripts.DataBind();
            PopulateProfileScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            gvScripts.DataSource = Call.ScriptModuleApi.GetImagingScripts();
            gvScripts.DataBind();
            PopulateProfileScripts();
        }

        protected void PopulateProfileScripts()
        {
            var profileScripts = Call.ImageProfileApi.GetScripts(ImageProfile.Id);
            foreach (GridViewRow row in gvScripts.Rows)
            {
                var runWhen = (DropDownList)row.FindControl("ddlRunWhen");
                PopulateRunWhen(runWhen);
                var dataKey = gvScripts.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                foreach (var profileScript in profileScripts)
                {
                    if (profileScript.ScriptModuleId == Convert.ToInt32(dataKey.Value))
                    {
                        runWhen.SelectedValue = profileScript.RunWhen.ToString();

                        var txtPriority = row.FindControl("txtPriority") as TextBox;
                        if (txtPriority != null)
                            txtPriority.Text = profileScript.Priority.ToString();
                    }
                }
            }


        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var deleteResult = Call.ImageProfileApi.RemoveProfileScripts(ImageProfile.Id);
            var checkedCount = 0;
            foreach (GridViewRow row in gvScripts.Rows)
            {
                var runWhen = (DropDownList)row.FindControl("ddlRunWhen");
                if (runWhen.Text == "Disabled")
                    continue;

                checkedCount++;
                var dataKey = gvScripts.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

                var profileScript = new EntityImageProfileScript
                {
                    ScriptModuleId = Convert.ToInt32(dataKey.Value),
                    ProfileId = ImageProfile.Id,
                    RunWhen = (EnumProfileScript.RunWhen)Enum.Parse(typeof(EnumProfileScript.RunWhen), runWhen.SelectedValue)
                };
                var txtPriority = row.FindControl("txtPriority") as TextBox;
                if (txtPriority != null)
                    if (!string.IsNullOrEmpty(txtPriority.Text))
                        profileScript.Priority = Convert.ToInt32(txtPriority.Text);
                EndUserMessage = Call.ImageProfileScriptApi.Post(profileScript).Success
                    ? "Successfully Updated Image Profile"
                    : "Could Not Update Image Profile";
            }
            if (checkedCount == 0)
            {
                EndUserMessage = deleteResult
                    ? "Successfully Updated Image Profile"
                    : "Could Not Update Image Profile";
            }
        }

        protected void PopulateRunWhen(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumProfileScript.RunWhen));
            ddl.DataBind();
        }
    }
}