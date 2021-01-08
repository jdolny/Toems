using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.computers
{
    public partial class modulerunner : BasePages.Computers
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

            var filter = new DtoModuleSearchFilter();
            filter.Limit = Int32.MaxValue;
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

        protected void chkFilter_OnCheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void btnRunModule_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;

            var gvRow = (GridViewRow)control.Parent.Parent;
            var dataKey = gvModules.DataKeys[gvRow.RowIndex];
            var moduleGuid = gvRow.Cells[5].Text.ToString();

            Call.ComputerApi.ClearLastSocketResult(ComputerEntity.Id);
            Call.ComputerApi.RunModule(ComputerEntity.Id, moduleGuid);
            var counter = 0;
            while (counter < 10)
            {
                var lastSocketResult = Call.ComputerApi.GetLastSocketResult(ComputerEntity.Id);
                if (!string.IsNullOrEmpty(lastSocketResult))
                {
                    EndUserMessage = lastSocketResult;
                    break;
                }
                if (counter == 9)
                {
                    EndUserMessage = "Could Not Start Instant Module";
                    return;
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }

        }
    }
}