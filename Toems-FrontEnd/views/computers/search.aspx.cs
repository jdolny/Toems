using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.computers
{
    class BoundFieldOrder
    {
        public BoundField Field;
        public int Order;
    }

    public partial class search : BasePages.Computers
    {
        EntityToemsUserOptions _entityToemsUserOptions { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            _entityToemsUserOptions = Call.ToemsUserApi.GetUserComputerOptions(ToemsCurrentUser.Id);
            if (!IsPostBack)
            {
                PopulateCategories();
            }
            
            PopulateGrid();
        }
        private void SessionChecks()
        {
            var ids = new List<int>();
            
            foreach (GridViewRow row in gvComputers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvComputers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var id = Convert.ToInt32(dataKey.Value);
                ids.Add(id);
            }
            Session["checked_ids"] = ids;
        }

        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //PopulateGrid();
        }

        private void PopulateCategories()
        {
            selectCategory.DataSource = Call.CategoryApi.Get().Select(x => x.Name).ToList();
            selectCategory.DataBind();
        }

        protected void PopulateGrid()
        {
            SessionChecks();
            var filter = new DtoSearchFilterCategories();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = txtSearch.Text;
            filter.CategoryType = ddlCatType.Text;
            filter.Categories = SelectedCategories();


            var listOfComputers = Call.ComputerApi.Search(filter);
            gvComputers.DataSource = listOfComputers;

            BuildDynamicGrid();
            gvComputers.DataBind();

            AddLinkcolumn();
            lblTotal.Text = gvComputers.Rows.Count + " Result(s) / " + Call.ComputerApi.GetActiveCount() + " Computer(s)";

            var checks = (List<int>)Session["checked_ids"];
            Session.Remove("checked_ids");

            foreach (GridViewRow row in gvComputers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                var dataKey = gvComputers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var id = Convert.ToInt32(dataKey.Value);
                if(checks.Contains(id)) cb.Checked = true;
            }
        }
        
        private void BuildDynamicGrid()
        {
            if(_entityToemsUserOptions == null) return;

            var boundFields = new List<BoundFieldOrder>();
            if (!IsPostBack)
            {
                if (_entityToemsUserOptions.DescriptionEnabled)
                {
                    var boundFieldOrder = new BoundFieldOrder();
                    BoundField description = new BoundField();
                    description.DataField = "Description";
                    description.HeaderText = "Description";
                    description.SortExpression = "Description";
                    description.ItemStyle.CssClass = "width_200";

                    boundFieldOrder.Field = description;
                    boundFieldOrder.Order = _entityToemsUserOptions.DescriptionOrder;
                    boundFields.Add(boundFieldOrder);
                }

                if (_entityToemsUserOptions.LastCheckinEnabled)
                {
                    var boundFieldOrder = new BoundFieldOrder();
                    BoundField lastCheckin = new BoundField();
                    lastCheckin.DataField = "LastCheckinTime";
                    lastCheckin.HeaderText = "Last Checkin";
                    lastCheckin.SortExpression = "LastCheckinTime";
                    lastCheckin.ItemStyle.CssClass = "width_200";

                    boundFieldOrder.Field = lastCheckin;
                    boundFieldOrder.Order = _entityToemsUserOptions.LastCheckinOrder;
                    boundFields.Add(boundFieldOrder);
                }

                if (_entityToemsUserOptions.LastIpEnabled)
                {
                    var boundFieldOrder = new BoundFieldOrder();
                    BoundField lastIp = new BoundField();
                    lastIp.DataField = "LastIp";
                    lastIp.HeaderText = "Last IP";
                    lastIp.SortExpression = "LastIp";
                    lastIp.ItemStyle.CssClass = "width_200";

                    boundFieldOrder.Field = lastIp;
                    boundFieldOrder.Order = _entityToemsUserOptions.LastIpOrder;
                    boundFields.Add(boundFieldOrder);
                }

                if (_entityToemsUserOptions.ClientVersionEnabled) 
                {
                    var boundFieldOrder = new BoundFieldOrder();
                    BoundField clientVersion = new BoundField();
                    clientVersion.DataField = "ClientVersion";
                    clientVersion.HeaderText = "Client Version";
                    clientVersion.SortExpression = "ClientVersion";
                    clientVersion.ItemStyle.CssClass = "width_200";

                    boundFieldOrder.Field = clientVersion;
                    boundFieldOrder.Order = _entityToemsUserOptions.ClientVersionOrder;
                    boundFields.Add(boundFieldOrder);
                }

                if (_entityToemsUserOptions.LastUserEnabled)
                {
                    var boundFieldOrder = new BoundFieldOrder();
                    BoundField lastUser = new BoundField();
                    lastUser.DataField = "LastLoggedInUser";
                    lastUser.HeaderText = "Last User";
                    lastUser.SortExpression = "LastLoggedInUser";
                    lastUser.ItemStyle.CssClass = "width_200";

                    boundFieldOrder.Field = lastUser;
                    boundFieldOrder.Order = _entityToemsUserOptions.LastUserOrder;
                    boundFields.Add(boundFieldOrder);
                }

                if (_entityToemsUserOptions.ProvisionDateEnabled)
                {
                    var boundFieldOrder = new BoundFieldOrder();
                    BoundField provisionDate = new BoundField();
                    provisionDate.DataField = "ProvisionedTime";
                    provisionDate.HeaderText = "Provision Date";
                    provisionDate.SortExpression = "ProvisionedTime";
                    provisionDate.ItemStyle.CssClass = "width_200";

                    boundFieldOrder.Field = provisionDate;
                    boundFieldOrder.Order = _entityToemsUserOptions.ProvisionDateOrder;
                    boundFields.Add(boundFieldOrder);
                }

                if (_entityToemsUserOptions.StatusEnabled)
                {
                    var boundFieldOrder = new BoundFieldOrder();
                    BoundField status = new BoundField();
                    status.DataField = "Status";
                    status.HeaderText = "Status";
                    status.SortExpression = "Status";
                    status.ItemStyle.CssClass = "width_200";

                    boundFieldOrder.Field = status;
                    boundFieldOrder.Order = _entityToemsUserOptions.StatusOrder;
                    boundFields.Add(boundFieldOrder);
                }

                var fieldCounter = 0;
                foreach (var field in boundFields.OrderBy(x => x.Order))
                {
                    fieldCounter++;
                    if (fieldCounter == boundFields.Count)
                        field.Field.ItemStyle.CssClass = "";
                    gvComputers.Columns.Add(field.Field);
                }

            }

            BoundField linkHolder = new BoundField();
            gvComputers.Columns.Add(linkHolder);
        }

        protected void search_Changed(object sender, EventArgs e)
        {

        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
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
                case "Status":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Status).ToList()
                        : listModules.OrderBy(h => h.Status).ToList();
                    break;

            }

            gvComputers.DataSource = listModules;
            gvComputers.DataBind();
        }

        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
            var action = (string) Session["action"];
            Session.Remove("action");
            var count = 0;
            switch (action)
            {
                case "delete":
                    foreach (GridViewRow row in gvComputers.Rows)
                    {
                        var cb = (CheckBox) row.FindControl("chkSelector");
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
                        var cb = (CheckBox) row.FindControl("chkSelector");
                        if (cb == null || !cb.Checked) continue;
                        var dataKey = gvComputers.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (Call.ComputerApi.Archive(Convert.ToInt32(dataKey.Value)).Success)
                            count++;
                    }
                    EndUserMessage = "Archived " + count + " Computer(s)";
                    break;
            }
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

        protected void AddLinkcolumn()
        {
            foreach (GridViewRow row in gvComputers.Rows)
            {
                var dataKey = gvComputers.DataKeys[row.RowIndex];
                LinkButton lnkView = new LinkButton();
                lnkView.ID = "lnkView";
                lnkView.Text = "View me";
                lnkView.Click += ViewDetails;
                lnkView.CommandArgument = dataKey.Value.ToString();
                row.Cells[gvComputers.Columns.Count - 1].Controls.Add(lnkView);
            }
            
        }

        protected void ViewDetails(object sender, EventArgs e)
        {
            LinkButton lnkView = (sender as LinkButton);
            string id = lnkView.CommandArgument;
            EndUserMessage = "clicked  " + id;
        }
    }
}