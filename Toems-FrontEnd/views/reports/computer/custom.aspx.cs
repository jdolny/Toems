using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.reports.computer
{
    public partial class custom : BasePages.Report
    {
        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            var dt = (DataTable)ViewState["nbiEntries"];
            var queries = new List<DtoCustomComputerQuery>();
            int currentRow = 0;
            if (dt.Rows.Count > 0)
            {

                foreach (GridViewRow row in gvNetBoot.Rows)
                {
                    currentRow++;
                    var query = new DtoCustomComputerQuery();
                    query.IncludeArchived = chkArchived.Checked;
                    query.IncludePreProvisioned = chkPre.Checked;
                    query.GroupBy = ddlGroupBy.SelectedItem.Text;
                    query.AndOr = ((Label)row.FindControl("lblAndOr")).Text;
                    if (dt.Rows.Count > 1 && query.AndOr.Equals("Not"))
                    {
                        EndUserMessage = "Not Can Only Be Used In A Single Query";
                        return;
                    }

                    if (currentRow > 1 && query.AndOr.Equals("Not"))
                    {
                        EndUserMessage = "Not Can Only Be Used In The First Query";
                        return;
                    }
                    query.LeftParenthesis = ((Label)row.FindControl("lblLeftPar")).Text;
                    query.Table = ((Label)row.FindControl("lblTable")).Text;
                    query.Field = ((Label)row.FindControl("lblField")).Text;
                    query.Operator = ((Label)row.FindControl("lblOperator")).Text;
                    query.Value = ((Label)row.FindControl("lblValue")).Text;
                    query.RightParenthesis = ((Label)row.FindControl("lblRightPar")).Text;

                    queries.Add(query);
                }

            }
            var result = Call.ReportApi.GetCustomComputer(queries);
            gvResult.DataSource = result;
            gvResult.DataBind();
            ExportDataSetToCSV(result,"inventory.csv");
        }

        protected void BindGrid()
        {
            DataTable dt;
            if (ViewState["nbiEntries"] != null)
            {
                dt = (DataTable)ViewState["nbiEntries"];
                if (dt.Rows.Count > 0)
                {

                    gvNetBoot.DataSource = dt;
                    gvNetBoot.DataBind();


                }
            }
            else
            {

                dt = new DataTable();
                dt.Columns.Add("AndOr");
                dt.Columns.Add("LeftParenthesis");
                dt.Columns.Add("Table");
                dt.Columns.Add("Field");
                dt.Columns.Add("Operator");
                dt.Columns.Add("Value");
                dt.Columns.Add("RightParenthesis");
                var dataRow = dt.NewRow();
                dt.Rows.Add(dataRow);
                gvNetBoot.DataSource = dt;
                gvNetBoot.DataBind();
                gvNetBoot.Rows[0].Cells.Clear();

                var emptyTable = new DataTable();
                emptyTable.Columns.Add("AndOr");
                emptyTable.Columns.Add("LeftParenthesis");
                emptyTable.Columns.Add("Table");
                emptyTable.Columns.Add("Field");
                emptyTable.Columns.Add("Operator");
                emptyTable.Columns.Add("Value");
                emptyTable.Columns.Add("RightParenthesis");
                emptyTable.Columns.Add("Id");
                emptyTable.Columns.Add("GroupId");
                emptyTable.Columns.Add("Order");
                emptyTable.Clear();
                ViewState["nbiEntries"] = emptyTable;

            }

        }

        private void PopulateTables(DropDownList ddl)
        {
            ddl.Items.Insert(0, new ListItem("Computer", "Computer"));
            ddl.Items.Insert(1, new ListItem("System", "System"));
            ddl.Items.Insert(2, new ListItem("Bios", "Bios"));
            ddl.Items.Insert(3, new ListItem("Hard Drive", "Hard Drive"));
            ddl.Items.Insert(4, new ListItem("OS", "OS"));
            ddl.Items.Insert(5, new ListItem("Printer", "Printer"));
            ddl.Items.Insert(6, new ListItem("Processor", "Processor"));
            ddl.Items.Insert(7, new ListItem("Application", "Application"));
            ddl.Items.Insert(8, new ListItem("Windows Update", "Windows Update"));
            ddl.Items.Insert(9, new ListItem("Firewall", "Firewall"));
            ddl.Items.Insert(10, new ListItem("AntiVirus", "AntiVirus"));
            ddl.Items.Insert(11, new ListItem("BitLocker", "BitLocker"));
            ddl.Items.Insert(12, new ListItem("Logical Volumes", "Logical Volumes"));
            ddl.Items.Insert(13, new ListItem("Network Adapters", "Network Adapters"));
            ddl.Items.Insert(14, new ListItem("Certificates", "Certificates"));
            ddl.Items.Insert(15, new ListItem("Category", "Category"));
            ddl.Items.Insert(16, new ListItem("Gpu", "Gpu"));

            var counter = 16;
            var customInventories = Call.ScriptModuleApi.GetAllWithInventory();
            foreach (var ci in customInventories)
            {
                counter++;
                ddl.Items.Insert(counter, new ListItem(ci.Name + "(ci_" + ci.Id + ")", "(ci_" + ci.Id + ")"));
            }

            var computerAttributes = Call.CustomAttributeApi.GetForBuiltInComputers();
            foreach (var ca in computerAttributes)
            {
                counter++;
                ddl.Items.Insert(counter, new ListItem(ca.Name + "(ca_" + ca.Id + ")", "(ca_" + ca.Id + ")"));
            }
        }

        protected void gv_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            DropDownList ddlTable = null;
            DropDownList ddlField = null;

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                ddlTable = e.Row.FindControl("ddlTable") as DropDownList;
                PopulateTables(ddlTable);
                ddlField = e.Row.FindControl("ddlField") as DropDownList;
                PopulateFields(ddlTable, ddlField);
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                ddlTable = e.Row.FindControl("ddlTable") as DropDownList;
                if (ddlTable != null)
                {
                    PopulateTables(ddlTable);
                    ddlTable.SelectedValue = ((DataRowView)e.Row.DataItem).Row.Field<string>("Table");
                    ddlField = e.Row.FindControl("ddlField") as DropDownList;
                    PopulateFields(ddlTable, ddlField);
                    ddlField.SelectedValue = ((DataRowView)e.Row.DataItem).Row.Field<string>("Field");

                }
            }


        }


        protected void btnAdd1_OnClick(object sender, EventArgs e)
        {
            var gvRow = (GridViewRow)(sender as Control).Parent.Parent;
            var andOr = ((DropDownList)gvRow.FindControl("ddlAndOr")).Text;
            var leftPar = ((DropDownList)gvRow.FindControl("ddlLeftPar")).Text;
            var table = ((DropDownList)gvRow.FindControl("ddlTable")).SelectedValue;
            var field = ((DropDownList)gvRow.FindControl("ddlField")).Text;
            var op = ((DropDownList)gvRow.FindControl("ddlOperator")).Text;
            var value = ((TextBox)gvRow.FindControl("txtValue")).Text;
            var rightPar = ((DropDownList)gvRow.FindControl("ddlRightPar")).Text;
            var dt = (DataTable)ViewState["nbiEntries"];
            var dataRow = dt.NewRow();
            dataRow[0] = andOr;
            dataRow[1] = leftPar;
            dataRow[2] = table;
            dataRow[3] = field;
            dataRow[4] = op;
            dataRow[5] = value;
            dataRow[6] = rightPar;
            dt.Rows.Add(dataRow);
            ViewState["nbiEntries"] = dt;
            BindGrid();
            PopulateGroupBy();
        }

        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            gvNetBoot.EditIndex = -1;
            BindGrid();
            PopulateGroupBy();
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var dt = (DataTable)ViewState["nbiEntries"];
            dt.Rows[e.RowIndex].Delete();
            gvNetBoot.DataSource = dt;
            gvNetBoot.DataBind();
            if (gvNetBoot.Rows.Count == 0)
            {
                ViewState["nbiEntries"] = null;
                BindGrid();
            }
            PopulateGroupBy();
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            gvNetBoot.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var gvRow = gvNetBoot.Rows[e.RowIndex];
            var dt = (DataTable)ViewState["nbiEntries"];

            dt.Rows[e.RowIndex]["AndOr"] = ((DropDownList)gvRow.FindControl("ddlAndOr")).Text;
            dt.Rows[e.RowIndex]["LeftParenthesis"] = ((DropDownList)gvRow.FindControl("ddlLeftPar")).Text;
            dt.Rows[e.RowIndex]["Table"] = ((DropDownList)gvRow.FindControl("ddlTable")).Text;
            dt.Rows[e.RowIndex]["Field"] = ((DropDownList)gvRow.FindControl("ddlField")).Text;
            dt.Rows[e.RowIndex]["Operator"] = ((DropDownList)gvRow.FindControl("ddlOperator")).Text;
            dt.Rows[e.RowIndex]["Value"] = ((TextBox)gvRow.FindControl("txtValue")).Text;
            dt.Rows[e.RowIndex]["RightParenthesis"] = ((DropDownList)gvRow.FindControl("ddlRightPar")).Text;

            ViewState["nbiEntries"] = dt;
            gvNetBoot.EditIndex = -1;
            BindGrid();
            PopulateGroupBy();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
            else
            {
                var dt = (DataTable)ViewState["nbiEntries"];
                if (dt.Rows.Count == 0)
                {
                    gvNetBoot.Rows[0].Cells.Clear();
                }
            }



        }


        protected void btnTestQuery_OnClick(object sender, EventArgs e)
        {
            var dt = (DataTable)ViewState["nbiEntries"];
            var queries = new List<DtoCustomComputerQuery>();
            int currentRow = 0;
            if (dt.Rows.Count > 0)
            {

                foreach (GridViewRow row in gvNetBoot.Rows)
                {
                    currentRow++;
                    var query = new DtoCustomComputerQuery();
                    query.IncludeArchived = chkArchived.Checked;
                    query.IncludePreProvisioned = chkPre.Checked;
                    query.GroupBy = ddlGroupBy.SelectedItem.Text;
                    query.AndOr = ((Label)row.FindControl("lblAndOr")).Text;
                    if (dt.Rows.Count > 1 && query.AndOr.Equals("Not"))
                    {
                        EndUserMessage = "Not Can Only Be Used In A Single Query";
                        return;
                    }

                    if (currentRow > 1 && query.AndOr.Equals("Not"))
                    {
                        EndUserMessage = "Not Can Only Be Used In The First Query";
                        return;
                    }
                    query.LeftParenthesis = ((Label)row.FindControl("lblLeftPar")).Text;
                    query.Table = ((Label)row.FindControl("lblTable")).Text;
                    query.Field = ((Label)row.FindControl("lblField")).Text;
                    query.Operator = ((Label)row.FindControl("lblOperator")).Text;
                    query.Value = ((Label)row.FindControl("lblValue")).Text;
                    query.RightParenthesis = ((Label)row.FindControl("lblRightPar")).Text;

                    queries.Add(query);
                }

            }
            var result = Call.ReportApi.GetCustomComputer(queries);
            gvResult.DataSource = result;
            gvResult.DataBind();
            lblTotal.Text = gvResult.Rows.Count + " Result(s)";
        }

        protected void ddlTable_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var gvRow = (GridViewRow)(sender as Control).Parent.Parent;
            var table = ((DropDownList)gvRow.FindControl("ddlTable"));
            var field = ((DropDownList)gvRow.FindControl("ddlField"));
            PopulateFields(table, field);
            
        }

        private void PopulateGroupBy()
        {
            ddlGroupBy.Items.Clear();
            ddlGroupBy.Items.Insert(0, new ListItem("", ""));
            ddlGroupBy.Items.Insert(1, new ListItem("Computer.computer_name", "Computer.computer_name"));
            var dt = (DataTable) ViewState["nbiEntries"];
            int indexCount = 1;
            if (dt.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvNetBoot.Rows)
                {
                    var table = ((Label) row.FindControl("lblTable")).Text;
                    var field = ((Label) row.FindControl("lblField")).Text;
                    if (table.Equals("Computer") && field.Equals("computer_name")) continue;
                    indexCount++;
                    ddlGroupBy.Items.Insert(indexCount, new ListItem(table + "." + field, table + "." + field));
                }
            }
        }

        private void PopulateFields(DropDownList ddlTable, DropDownList ddlField)
        {
            ddlField.Items.Clear();
            if (ddlTable.Text == "Computer")
            {
                ddlField.Items.Insert(0, new ListItem("computer_name", "computer_name"));
                ddlField.Items.Insert(1, new ListItem("provisioned_time_local", "provisioned_time_local"));
                ddlField.Items.Insert(2, new ListItem("last_checkin_time_local", "last_checkin_time_local"));
                ddlField.Items.Insert(3, new ListItem("last_ip", "last_ip"));
                ddlField.Items.Insert(4, new ListItem("is_ad_sync", "is_ad_sync"));
                ddlField.Items.Insert(5, new ListItem("client_version", "client_version"));
                ddlField.Items.Insert(6, new ListItem("last_inventory_time_local", "last_inventory_time_local"));
                ddlField.Items.Insert(7, new ListItem("ad_disabled", "ad_disabled"));
                ddlField.Items.Insert(8, new ListItem("provision_status", "provision_status"));
            }
            else if (ddlTable.Text == "System")
            {
                ddlField.Items.Insert(0, new ListItem("manufacturer", "manufacturer"));
                ddlField.Items.Insert(1, new ListItem("model", "model"));
                ddlField.Items.Insert(2, new ListItem("domain", "domain"));
                ddlField.Items.Insert(3, new ListItem("workgroup", "workgroup"));
                ddlField.Items.Insert(4, new ListItem("memory", "memory"));
            }
            else if (ddlTable.Text == "Bios")
            {
                ddlField.Items.Insert(0, new ListItem("serial_number", "serial_number"));
                ddlField.Items.Insert(1, new ListItem("version", "version"));
                ddlField.Items.Insert(2, new ListItem("sm_bios_version", "sm_bios_version"));
            }
            else if (ddlTable.Text == "Hard Drive")
            {
                ddlField.Items.Insert(0, new ListItem("model", "model"));
                ddlField.Items.Insert(1, new ListItem("firmware", "firmware"));
                ddlField.Items.Insert(2, new ListItem("serial_number", "serial_number"));
                ddlField.Items.Insert(3, new ListItem("size", "size"));
                ddlField.Items.Insert(4, new ListItem("smart_status", "smart_status"));
            }
            else if (ddlTable.Text == "Gpu")
            {
                ddlField.Items.Insert(0, new ListItem("computer_gpu_name", "computer_gpu_name"));
                ddlField.Items.Insert(1, new ListItem("computer_gpu_ram", "computer_gpu_ram"));
            }

            else if (ddlTable.Text == "OS")
            {
                ddlField.Items.Insert(0, new ListItem("name", "name"));
                ddlField.Items.Insert(1, new ListItem("version", "version"));
                ddlField.Items.Insert(2, new ListItem("build", "build"));
                ddlField.Items.Insert(3, new ListItem("arch", "arch"));
                ddlField.Items.Insert(4, new ListItem("sp_major", "sp_major"));
                ddlField.Items.Insert(5, new ListItem("sp_minor", "sp_minor"));
                ddlField.Items.Insert(6, new ListItem("release_id", "release_id"));
                ddlField.Items.Insert(7, new ListItem("uac_status", "uac_status"));
                ddlField.Items.Insert(8, new ListItem("local_time_zone", "local_time_zone"));
                ddlField.Items.Insert(9, new ListItem("location_enabled", "location_enabled"));
                ddlField.Items.Insert(10, new ListItem("last_location_update_utc", "last_location_update_utc"));
                ddlField.Items.Insert(11, new ListItem("update_server", "update_server"));
                ddlField.Items.Insert(11, new ListItem("update_server_target_group", "update_server_target_group"));
            }
            else if (ddlTable.Text == "Printer")
            {
                ddlField.Items.Insert(0, new ListItem("name", "name"));
                ddlField.Items.Insert(1, new ListItem("driver_name", "driver_name"));
                ddlField.Items.Insert(2, new ListItem("is_local", "is_local"));
                ddlField.Items.Insert(3, new ListItem("is_network", "is_network"));
                ddlField.Items.Insert(4, new ListItem("share_name", "share_name"));
                ddlField.Items.Insert(5, new ListItem("system_name", "system_name"));
            }
            else if (ddlTable.Text == "Processor")
            {
                ddlField.Items.Insert(0, new ListItem("name", "name"));
                ddlField.Items.Insert(1, new ListItem("clock_speed", "clock_speed"));
                ddlField.Items.Insert(2, new ListItem("cores", "cores"));
            }
            else if (ddlTable.Text == "Application")
            {
                ddlField.Items.Insert(0, new ListItem("name", "name"));
                ddlField.Items.Insert(1, new ListItem("version", "version"));
                ddlField.Items.Insert(2, new ListItem("major", "major"));
                ddlField.Items.Insert(3, new ListItem("minor", "minor"));
                ddlField.Items.Insert(4, new ListItem("build", "build"));
                ddlField.Items.Insert(5, new ListItem("revision", "revision"));
            }

            else if (ddlTable.Text == "Windows Update")
            {
                ddlField.Items.Insert(0, new ListItem("title", "title"));
                ddlField.Items.Insert(1, new ListItem("install_date", "install_date"));
                ddlField.Items.Insert(2, new ListItem("is_installed", "is_installed"));
            }
            else if (ddlTable.Text == "Firewall")
            {
                ddlField.Items.Insert(0, new ListItem("domain_enabled", "domain_enabled"));
                ddlField.Items.Insert(1, new ListItem("private_enabled", "private_enabled"));
                ddlField.Items.Insert(2, new ListItem("public_enabled", "public_enabled"));
            }
            else if (ddlTable.Text == "AntiVirus")
            {
                ddlField.Items.Insert(0, new ListItem("display_name", "display_name"));
                ddlField.Items.Insert(1, new ListItem("provider", "provider"));
                ddlField.Items.Insert(2, new ListItem("rt_scanner", "rt_scanner"));
                ddlField.Items.Insert(3, new ListItem("definition_status", "definition_status"));
                ddlField.Items.Insert(4, new ListItem("product_state", "product_state"));
            }
            else if (ddlTable.Text == "BitLocker")
            {
                ddlField.Items.Insert(0, new ListItem("drive_letter", "drive_letter"));
                ddlField.Items.Insert(1, new ListItem("status", "status"));
            }
            else if (ddlTable.Text == "Logical Volumes")
            {
                ddlField.Items.Insert(0, new ListItem("drive", "drive"));
                ddlField.Items.Insert(1, new ListItem("free_space_gb", "free_space_gb"));
                ddlField.Items.Insert(2, new ListItem("free_space_percent", "free_space_percent"));
                ddlField.Items.Insert(3, new ListItem("size_gb", "size_gb"));
            }
            else if (ddlTable.Text == "Network Adapters")
            {
                ddlField.Items.Insert(0, new ListItem("nic_name", "nic_name"));
                ddlField.Items.Insert(1, new ListItem("nic_description", "nic_description"));
                ddlField.Items.Insert(2, new ListItem("nic_type", "nic_type"));
                ddlField.Items.Insert(3, new ListItem("nic_mac", "nic_mac"));
                ddlField.Items.Insert(4, new ListItem("nic_status", "nic_status"));
                ddlField.Items.Insert(5, new ListItem("nic_speed", "nic_speed"));
                ddlField.Items.Insert(6, new ListItem("nic_ips", "nic_ips"));
                ddlField.Items.Insert(7, new ListItem("nic_gateways", "nic_gateways"));
            }
            else if (ddlTable.Text == "Certificates")
            {
                ddlField.Items.Insert(0, new ListItem("store", "store"));
                ddlField.Items.Insert(1, new ListItem("subject", "subject"));
                ddlField.Items.Insert(2, new ListItem("friendlyname", "friendlyname"));
                ddlField.Items.Insert(3, new ListItem("thumbprint", "thumbprint"));
                ddlField.Items.Insert(4, new ListItem("serial", "serial"));
                ddlField.Items.Insert(5, new ListItem("notbefore_utc", "notbefore_utc"));
                ddlField.Items.Insert(6, new ListItem("notafter_utc", "notafter_utc"));
            }
            else if (ddlTable.Text == "Category")
            {
                ddlField.Items.Insert(0, new ListItem("category_name", "category_name"));
            }
            else
            {
                ddlField.Items.Insert(0, new ListItem("value", "value"));
            }


        }
    }
}