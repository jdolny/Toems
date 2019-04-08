using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.policies
{
    public partial class availablemodules : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                chkScript.Checked = true;
                chkCommand.Checked = true;
                chkSoftware.Checked = true;
                chkFile.Checked = true;
                chkPrinter.Checked = true;
                chkWu.Checked = true;
                chkMessage.Checked = true;
                PopulateGrid();
            }
        }

        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var limit = 0;
            limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);

            var filter = new DtoModuleSearchFilter();
            filter.Limit = limit;
            filter.Searchstring = txtSearch.Text;
            filter.IncludePrinter = chkPrinter.Checked;
            filter.IncludeSoftware = chkSoftware.Checked;
            filter.IncludeCommand = chkCommand.Checked;
            filter.IncludeFileCopy = chkFile.Checked;
            filter.IncludeScript = chkScript.Checked;
            filter.IncludeWu = chkWu.Checked;
            filter.IncludeUnassigned = chkUnassigned.Checked;
            filter.IncludeMessage = chkMessage.Checked;

            var modules = Call.PolicyApi.GetAllModules(filter);
            gvModules.DataSource = modules;
            gvModules.DataBind();

           // lblTotal.Text = gvModules.Rows.Count + " Result(s) / " + Call.PolicyApi.GetCount() + " Policy(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            PopulateGrid();

            var listModules = (List<DtoModule>)gvModules.DataSource;
            switch (e.SortExpression)
            {
                case "Name":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.Name).ToList()
                        : listModules.OrderBy(h => h.Name).ToList();
                    break;
                case "ModuleType":
                    listModules = GetSortDirection(e.SortExpression) == "Desc"
                        ? listModules.OrderByDescending(h => h.ModuleType).ToList()
                        : listModules.OrderBy(h => h.ModuleType).ToList();
                    break;

            }

            gvModules.DataSource = listModules;
            gvModules.DataBind();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvModules);
        }

        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {
            var listPolicyModules = new List<EntityPolicyModules>();

            foreach (GridViewRow row in gvModules.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (cb == null || !cb.Checked) continue;
                var dataKey = gvModules.DataKeys[row.RowIndex];
                if (dataKey == null) continue;
                var policyModule = new EntityPolicyModules();
                policyModule.ModuleId = Convert.ToInt32(dataKey.Value);
                policyModule.PolicyId = Policy.Id;
                policyModule.Guid = row.Cells[5].Text;
                policyModule.ModuleType = (EnumModule.ModuleType)Enum.Parse(typeof(EnumModule.ModuleType), row.Cells[4].Text);
                listPolicyModules.Add(policyModule);
            }

            var result = Call.PolicyModulesApi.PostList(listPolicyModules);
            EndUserMessage = result.Success ? "Successfully Added Modules To Policy" : result.ErrorMessage;

        }

        protected void chkFilter_OnCheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}