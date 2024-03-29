﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_FrontEnd.views.reports.computer;

namespace Toems_FrontEnd.views.admin.toec
{
    public partial class createtargetlist : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                ddlListType.DataSource = Enum.GetNames(typeof(EnumToecDeployTargetList.ListType));
                ddlListType.DataBind();
            }
        }
    
        protected void PopulateOus()
        {
            BindTreeViewControl();

        }

        protected void PopulateAdGroups()
        {
            gvAdGroups.DataSource = Call.GroupApi.GetAdSecurityGroups();
            gvAdGroups.DataBind();
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {
            var toecTargetList = new EntityToecTargetList();
            toecTargetList.Name = txtName.Text;
            toecTargetList.Type = (EnumToecDeployTargetList.ListType)Enum.Parse(typeof(EnumToecDeployTargetList.ListType), ddlListType.SelectedValue);

            var selected = (EnumToecDeployTargetList.ListType)Enum.Parse(typeof(EnumToecDeployTargetList.ListType), ddlListType.SelectedValue);
            if (selected == EnumToecDeployTargetList.ListType.AdOU)
            {
                foreach (TreeNode node in treeOus.CheckedNodes)
                {
                    toecTargetList.GroupIds.Add(Convert.ToInt32(node.Value));
                }
            }
            else if (selected == EnumToecDeployTargetList.ListType.AdGroup)
            {
                foreach (GridViewRow row in gvAdGroups.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (cb == null || !cb.Checked) continue;
                    var dataKey = gvAdGroups.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;
                    toecTargetList.GroupIds.Add(Convert.ToInt32(dataKey.Value));
                }

            }
            else
            {
                var seperator = new string[] { "\r\n" };
                foreach (var obj in txtComputers.Text.Split(seperator, StringSplitOptions.RemoveEmptyEntries))
                {
                    toecTargetList.ComputerNames.Add(obj.Trim());
                }
            }

            var result = Call.ToecTargetListApi.Post(toecTargetList);
            if (!result.Success)
            {
                EndUserMessage = "Could Not create " + txtName.Text + " " + result.ErrorMessage;
            }
            else
            {
                EndUserMessage = "Successfully Created " + txtName.Text;

            }
        }

        protected void ddlListType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = (EnumToecDeployTargetList.ListType)Enum.Parse(typeof(EnumToecDeployTargetList.ListType), ddlListType.SelectedValue);
            if (selected == EnumToecDeployTargetList.ListType.AdOU)
            {
                ous.Visible = true;
                adGroups.Visible = false;
                computers.Visible = false;
                PopulateOus();
            }
            else if (selected == EnumToecDeployTargetList.ListType.AdGroup)
            {
                ous.Visible = false;
                adGroups.Visible = true;
                computers.Visible = false;
                PopulateAdGroups();
            }
            else
            {
                ous.Visible = false;
                adGroups.Visible = false;
                computers.Visible = true;
            }
        }

        private void BindTreeViewControl()
        {
            if (treeOus.Nodes.Count > 0) return;
            try
            {
                DataSet ds = GetDataSet();
                if (ds == null) return;
                DataRow[] rows = ds.Tables[0].Select("Convert(ParentId, 'System.Int32') =0");
                //DataRow[] rows = ds.Tables[0].Select("ParentId = 0"); // Get all parents nodes
                for (int i = 0; i < rows.Length; i++)
                {
                    TreeNode root = new TreeNode(rows[i]["Name"].ToString(), rows[i]["Id"].ToString());
                    root.SelectAction = TreeNodeSelectAction.Expand;
                    root.Expanded = true;

                    CreateNode(root, ds.Tables[0]);

                    treeOus.Nodes.Add(root);
                }
            }
            catch (Exception Ex) { throw Ex; }
        }

        public void CreateNode(TreeNode node, DataTable Dt)
        {
            DataRow[] rows = Dt.Select("Convert(ParentId, 'System.Int32') =" + node.Value);
            //DataRow[] rows = Dt.Select("ParentId = " + node.Value);
            if (rows.Length == 0) { return; }
            for (int i = 0; i < rows.Length; i++)
            {
                TreeNode childnode = new TreeNode(rows[i]["Name"].ToString(), rows[i]["Id"].ToString());
                childnode.SelectAction = TreeNodeSelectAction.Expand;
                node.ChildNodes.Add(childnode);
                CreateNode(childnode, Dt);
            }
        }
        private DataSet GetDataSet()
        {
            var groups = Call.GroupApi.GetAdGroups();

            if(groups == null || !groups.Any())
            {
                lblNoOu.Text = "No Organizational Units Were Found.  Ensure Active Directory Sync Is Configured.";
                return null;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("ParentId", typeof(string));

            foreach (var group in groups)
            {
                DataRow row = dt.NewRow();
                row["Id"] = group.Id;
                row["Name"] = group.Name;
                row["ParentId"] = group.ParentId;
                dt.Rows.Add(row);
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return ds;

        }



      
    }


}