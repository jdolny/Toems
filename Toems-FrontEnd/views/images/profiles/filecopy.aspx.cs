using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.images.profiles
{
    public partial class filecopy : BasePages.Images
    {
        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();
            var listFilesFolders = (List<EntityFileCopyModule>)gvFile.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listFilesFolders = GetSortDirection(e.SortExpression) == "Asc"
                        ? listFilesFolders.OrderBy(s => s.Name).ToList()
                        : listFilesFolders.OrderByDescending(s => s.Name).ToList();
                    break;
            }

            gvFile.DataSource = listFilesFolders;
            gvFile.DataBind();
            PopulateProfileScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            gvFile.DataSource = Call.FileCopyModuleApi.Get();
            gvFile.DataBind();

            PopulateProfileScripts();
        }

        protected void PopulateProfileScripts()
        {
            var profileFiles = Call.ImageProfileApi.GetFileCopy(ImageProfile.Id);
            foreach (GridViewRow row in gvFile.Rows)
            {
                var enabled = (CheckBox)row.FindControl("chkEnabled");
                var dataKey = gvFile.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                foreach (var profileFile in profileFiles)
                {
                    if (profileFile.FileCopyModuleId == Convert.ToInt32(dataKey.Value))
                    {
                        enabled.Checked = true;
                        var txtPriority = row.FindControl("txtPriority") as TextBox;
                        if (txtPriority != null)
                            txtPriority.Text = profileFile.Priority.ToString();
                        var txtPartition = row.FindControl("txtPartition") as TextBox;
                        if (txtPartition != null)
                            txtPartition.Text = profileFile.DestinationPartition;

                     
                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var deleteResult = Call.ImageProfileApi.RemoveProfileFileCopy(ImageProfile.Id);
            var checkedCount = 0;
            foreach (GridViewRow row in gvFile.Rows)
            {
                var enabled = (CheckBox)row.FindControl("chkEnabled");
                if (enabled == null) continue;
                if (!enabled.Checked) continue;
                checkedCount++;
                var dataKey = gvFile.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

                var profileFileFolder = new EntityImageProfileFileCopy
                {
                    FileCopyModuleId = Convert.ToInt32(dataKey.Value),
                    ProfileId = ImageProfile.Id
                };
                var txtPriority = row.FindControl("txtPriority") as TextBox;
                if (txtPriority != null)
                    if (!string.IsNullOrEmpty(txtPriority.Text))
                        profileFileFolder.Priority = Convert.ToInt32(txtPriority.Text);
                var txtPartition = row.FindControl("txtPartition") as TextBox;
                if (txtPartition != null)
                    profileFileFolder.DestinationPartition = txtPartition.Text;
              
                EndUserMessage = Call.ImageProfileFileCopyApi.Post(profileFileFolder).Success
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
    }
}