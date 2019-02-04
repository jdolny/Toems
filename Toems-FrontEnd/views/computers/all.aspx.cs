using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.computers
{
    public partial class all : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlStatus.DataSource = Enum.GetNames(typeof(EnumProvisionStatus.Status));
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0, new ListItem("", ""));
                ddlStatus.Items.Insert(1, new ListItem("Any Status", "Any Status"));
                ddlStatus.SelectedIndex = 1;
                PopulateGrid();
                PopulateCategories();
            }
        }

        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void PopulateCategories()
        {
            selectCategory.DataSource = Call.CategoryApi.Get().Select(x => x.Name).ToList();
            selectCategory.DataBind();
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilterAllComputers();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = txtSearch.Text;
            filter.Status = ddlStatus.Text;
            filter.State = ddlDisabled.Text;
            filter.CategoryType = ddlCatType.Text;
            filter.Categories = SelectedCategories();

            var listOfComputers = Call.ComputerApi.SearchAllComputers(filter);
            gvComputers.DataSource = listOfComputers;
            gvComputers.DataBind();

            lblTotal.Text = gvComputers.Rows.Count + " Result(s) / " + Call.ComputerApi.GetAllCount() + " Computer(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<EntityComputer>)gvComputers.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "LastCheckinTime":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.LastCheckinTime).ToList()
                        : listModules.OrderBy(h => h.LastCheckinTime).ToList();
                    break;
                case "LastIp":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.LastIp).ToList()
                        : listModules.OrderBy(h => h.LastIp).ToList();
                    break;
                case "ClientVersion":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.ClientVersion).ToList()
                        : listModules.OrderBy(h => h.ClientVersion).ToList();
                    break;
                case "ProvisionedTime":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.ProvisionedTime).ToList()
                        : listModules.OrderBy(h => h.ProvisionedTime).ToList();
                    break;
                case "ProvisionStatus":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.ProvisionStatus).ToList()
                        : listModules.OrderBy(h => h.ProvisionStatus).ToList();
                    break;
                case "AdDisabled":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.AdDisabled).ToList()
                        : listModules.OrderBy(h => h.AdDisabled).ToList();
                    break;
                case "IsAdSync":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.IsAdSync).ToList()
                        : listModules.OrderBy(h => h.IsAdSync).ToList();
                    break;

            }

            gvComputers.DataSource = listModules;
            gvComputers.DataBind();
        }

        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var action = (string)Session["action"];
            Session.Remove("action");
            var count = 0;
            switch (action)
            {
                case "delete":
                    foreach (GridViewRow row in gvComputers.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvComputers.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.ComputerApi.Delete(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Deleted " + count + " Computer(s)";
                    break;
                case "archive":
                    foreach (GridViewRow row in gvComputers.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvComputers.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.ComputerApi.Archive(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Archived " + count + " Computer(s)";
                    break;
            }
            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvComputers);
        }

        protected void btnArchive_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Archive Selected Computers?";
            Session["action"] = "archive";
            DisplayConfirm();
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete Selected Computers?";
            Session["action"] = "delete";
            DisplayConfirm();
        }

        protected void ddlStatus_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void ddlDisabled_OnSelectedIndexChanged(object sender, EventArgs e)
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
        protected void btnRestore_OnClick(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                var gvRow = (GridViewRow)control.Parent.Parent;
                var dataKey = gvComputers.DataKeys[gvRow.RowIndex];
                if (dataKey != null)
                {
                    var computer = Call.ComputerApi.Get(Convert.ToInt32(dataKey.Value));
                    if (computer.ProvisionStatus != EnumProvisionStatus.Status.Archived)
                    {
                        EndUserMessage = "Only Archived Computers Can Be Restored";
                        return;
                    }

                    var result = Call.ComputerApi.Restore(Convert.ToInt32(dataKey.Value));
                    EndUserMessage = result.Success ? "Successfully Restored Computer" : result.ErrorMessage;
                    PopulateGrid();
                }
            }
        }
    }
}