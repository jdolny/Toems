using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.groups
{
    public partial class oubrowser : BasePages.Groups
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTreeViewControl();
            }
        }
        private void BindTreeViewControl()
        {
            try
            {
                DataSet ds = GetDataSet();
                DataRow[] rows = ds.Tables[0].Select("Convert(ParentId, 'System.Int32') =0");
                //DataRow[] rows = ds.Tables[0].Select("ParentId = 0"); // Get all parents nodes
                for (int i = 0; i < rows.Length; i++)
                {
                    TreeNode root = new TreeNode(rows[i]["Name"].ToString(), rows[i]["Id"].ToString());
                    root.SelectAction = TreeNodeSelectAction.Expand;
                    root.NavigateUrl = "~/views/groups/general.aspx?groupId=" + rows[i]["Id"];
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
                childnode.NavigateUrl = "~/views/groups/general.aspx?groupId=" + rows[i]["Id"];
                CreateNode(childnode, Dt);
            }
        }
        private DataSet GetDataSet()
        {
            var groups = Call.GroupApi.GetAdGroups();
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