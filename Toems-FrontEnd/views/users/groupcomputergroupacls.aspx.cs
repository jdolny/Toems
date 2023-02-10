using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.users
{
    public partial class groupcomputergroupacls : BasePages.Users
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateGrid();
            }
        }

        protected void PopulateGrid()
        {
            chkEnableGroup.Checked = ToemsUserGroup.EnableComputerGroupAcls;

            var filter = new DtoSearchFilterCategories();
            filter.Limit = int.MaxValue;
            filter.IncludeOus = true;
            var listOfGroups = Call.GroupApi.Search(filter);
            gvGroups.DataSource = listOfGroups;
            gvGroups.DataBind();

            var managedGroups = Call.UserGroupApi.GetManagedGroupIds(ToemsUserGroup.Id).ToList();
            foreach (GridViewRow row in gvGroups.Rows)
            {
                if (managedGroups.Contains(Convert.ToInt32(gvGroups.DataKeys[row.RowIndex].Value)))
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (cb != null) cb.Checked = true;
                }
            }
     
        }


        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<DtoGroupWithCount>)gvGroups.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "Type":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Type).ToList()
                        : listModules.OrderBy(h => h.Type).ToList();
                    break;
                case "MemberCount":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.MemberCount).ToList()
                        : listModules.OrderBy(h => h.MemberCount).ToList();
                    break;

            }

            gvGroups.DataSource = listModules;
            gvGroups.DataBind();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvGroups);
        }

        protected void chkLdapGroups_CheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            ToemsUserGroup.EnableComputerGroupAcls = chkEnableGroup.Checked;
            var result = Call.UserGroupApi.Put(ToemsUserGroup.Id, ToemsUserGroup);
            if (!result.Success)
            {
                EndUserMessage = "Could Not Set Group Management Status";
                return;
            }

            var memberships = (from GridViewRow row in gvGroups.Rows
                               let cb = (CheckBox)row.FindControl("chkSelector")
                               where cb != null && cb.Checked
                               select gvGroups.DataKeys[row.RowIndex]
             into dataKey
                               where dataKey != null
                               select new EntityUserGroupComputerGroups()
                               {
                                   GroupId = Convert.ToInt32(dataKey.Value),
                                   UserGroupId = ToemsUserGroup.Id
                               }).ToList();
            result = Call.UserGroupApi.UpdateGroupManagement(memberships, ToemsUserGroup.Id);
            EndUserMessage = result.Success
                ? "Successfully Updated Group Management"
                : "Could Not Update Group Management." + result.ErrorMessage;
        }
    }
}