using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.reports.asset
{
    public partial class custom : BasePages.Report
    {
        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            var dt = (DataTable)ViewState["nbiEntries"];
            var queries = new List<DtoCustomComputerQuery>();

            if (dt.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvNetBoot.Rows)
                {
                    var query = new DtoCustomComputerQuery();
                    query.IncludeArchived = chkArchived.Checked;
                    query.GroupBy = ddlGroupBy.SelectedItem.Text;
                    query.AndOr = ((Label)row.FindControl("lblAndOr")).Text;

                    query.LeftParenthesis = ((Label)row.FindControl("lblLeftPar")).Text;
                    query.Table = ((Label)row.FindControl("lblTable")).Text;
                    query.Field = ((Label)row.FindControl("lblField")).Text;
                    query.Operator = ((Label)row.FindControl("lblOperator")).Text;
                    query.Value = ((Label)row.FindControl("lblValue")).Text;
                    query.RightParenthesis = ((Label)row.FindControl("lblRightPar")).Text;

                    queries.Add(query);
                }

            }
            var result = Call.ReportApi.GetCustomAsset(queries);
            gvResult.DataSource = result;
            gvResult.DataBind();
            ExportDataSetToCSV(result, "inventory.csv");
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
            ddl.Items.Insert(0, new ListItem("Asset", "Asset"));
            ddl.Items.Insert(1, new ListItem("Asset Type", "Asset Type"));
            var counter = 1;

            var computerAttributes = Call.CustomAttributeApi.Get();
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

            if (dt.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvNetBoot.Rows)
                {
                    var query = new DtoCustomComputerQuery();
                    query.IncludeArchived = chkArchived.Checked;
                    query.GroupBy = ddlGroupBy.SelectedItem.Text;
                    query.AndOr = ((Label)row.FindControl("lblAndOr")).Text;
                  
                    query.LeftParenthesis = ((Label)row.FindControl("lblLeftPar")).Text;
                    query.Table = ((Label)row.FindControl("lblTable")).Text;
                    query.Field = ((Label)row.FindControl("lblField")).Text;
                    query.Operator = ((Label)row.FindControl("lblOperator")).Text;
                    query.Value = ((Label)row.FindControl("lblValue")).Text;
                    query.RightParenthesis = ((Label)row.FindControl("lblRightPar")).Text;

                    queries.Add(query);
                }

            }
            var result = Call.ReportApi.GetCustomAsset(queries);
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
            ddlGroupBy.Items.Insert(1, new ListItem("Asset.asset_display_name", "Asset.asset_display_name"));
            var dt = (DataTable)ViewState["nbiEntries"];
            int indexCount = 1;
            if (dt.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvNetBoot.Rows)
                {
                    var table = ((Label)row.FindControl("lblTable")).Text;
                    var field = ((Label)row.FindControl("lblField")).Text;
                    if (table.Equals("Asset") && field.Equals("asset_display_name")) continue;
                    indexCount++;
                    ddlGroupBy.Items.Insert(indexCount, new ListItem(table + "." + field, table + "." + field));
                }
            }
        }

        private void PopulateFields(DropDownList ddlTable, DropDownList ddlField)
        {
            ddlField.Items.Clear();
            if (ddlTable.Text == "Asset")
            {
                ddlField.Items.Insert(0, new ListItem("asset_display_name", "asset_display_name"));
            }
            else if (ddlTable.Text == "Asset Type")
            {
                ddlField.Items.Insert(0, new ListItem("name", "name"));
            }
            else
            {
                ddlField.Items.Insert(0, new ListItem("value", "value"));
            }


        }
    }
}