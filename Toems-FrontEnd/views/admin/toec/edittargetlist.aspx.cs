using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.admin.toec
{
    public partial class edittargetlist : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                ddlListType.DataSource = Enum.GetNames(typeof(EnumToecDeployTargetList.ListType));
                ddlListType.DataBind();

                txtName.Text = ToecTargetList.Name;
                ddlListType.SelectedValue = ToecTargetList.Type.ToString();

                if (ToecTargetList.Type == EnumToecDeployTargetList.ListType.CustomList)
                {
                    ous.Visible = false;
                    adGroups.Visible = false;
                    computers.Visible = true;

                    var members = Call.ToecTargetListApi.GetMembers(ToecTargetList.Id);
                    foreach (var computer in members.OrderBy(x => x.Name))
                    {
                        txtComputers.Text += computer.Name + "\r\n";
                    }
                }
                else if (ToecTargetList.Type == EnumToecDeployTargetList.ListType.AdGroup)
                {
                    ous.Visible = false;
                    adGroups.Visible = true;
                    computers.Visible = false;
                    PopulateAdGroups();
                    foreach (GridViewRow row in gvAdGroups.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        if (cb == null) continue;
                        var dataKey = gvAdGroups.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;
                        if (ToecTargetList.GroupIds.Contains(Convert.ToInt32(dataKey.Value)))
                            cb.Checked = true;

                    }
                }
                else
                {
                    ous.Visible = true;
                    adGroups.Visible = false;
                    computers.Visible = false;
                    PopulateOus();

                  
                }
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

            ToecTargetList.Name = txtName.Text;
            ToecTargetList.GroupIds.Clear();
            ToecTargetList.Type = (EnumToecDeployTargetList.ListType)Enum.Parse(typeof(EnumToecDeployTargetList.ListType), ddlListType.SelectedValue);

            var selected = (EnumToecDeployTargetList.ListType)Enum.Parse(typeof(EnumToecDeployTargetList.ListType), ddlListType.SelectedValue);
            if (selected == EnumToecDeployTargetList.ListType.AdOU)
            {
                foreach (TreeNode node in treeOus.CheckedNodes)
                {
                    ToecTargetList.GroupIds.Add(Convert.ToInt32(node.Value));
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
                    ToecTargetList.GroupIds.Add(Convert.ToInt32(dataKey.Value));
                }

            }
            else
            {
                var seperator = new string[] { "\r\n" };
                foreach (var obj in txtComputers.Text.Split(seperator, StringSplitOptions.RemoveEmptyEntries))
                {
                    ToecTargetList.ComputerNames.Add(obj.Trim());
                }
            }

            var result = Call.ToecTargetListApi.Put(ToecTargetList.Id,ToecTargetList);
            if (!result.Success)
            {
                EndUserMessage = "Could Not Update " + txtName.Text + " " + result.ErrorMessage;
            }
            else
            {
                EndUserMessage = "Successfully Updated " + txtName.Text;

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
            if (treeOus.Nodes.Count > 0 ) return;
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
                    if (ToecTargetList.GroupIds.Contains(Convert.ToInt32(root.Value)))
                    {
                        root.Checked = true;


                    }
                    CreateNode(root, ds.Tables[0]);

                    treeOus.Nodes.Add(root);
                }
            }
            catch (Exception Ex) { throw Ex; }
        }

        private void ExpandParent(TreeNode node)
        {
            node.Expanded = true;

            var parent = node.Parent;
            if (parent != null)
            {
                parent.Expanded = true;
                ExpandParent(parent);
            }           
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
                if (ToecTargetList.GroupIds.Contains(Convert.ToInt32(childnode.Value)))
                {
                    childnode.Checked = true;
                    ExpandParent(node);

                }
                node.ChildNodes.Add(childnode);
                CreateNode(childnode, Dt);
            }
        }
        private DataSet GetDataSet()
        {
            var groups = Call.GroupApi.GetAdGroups();

            if (groups == null || !groups.Any())
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