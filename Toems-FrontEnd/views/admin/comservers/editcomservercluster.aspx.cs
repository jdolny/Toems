using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class editcomservercluster : BasePages.Admin
    {
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvServers);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {

            txtName.Text = ComServerCluster.Name;
            txtDescription.Text = ComServerCluster.Description;
            chkDefault.Checked = ComServerCluster.IsDefault;

            gvServers.DataSource = Call.ClientComServerApi.Get();
            gvServers.DataBind();

            var clusterServers = Call.ComServerClusterApi.GetClustServers(ComServerCluster.Id);
            foreach (GridViewRow row in gvServers.Rows)
            {
                var cb = (CheckBox) row.FindControl("chkSelector");
                var role = (DropDownList) row.FindControl("ddlRole");
                var isImagingServer = (CheckBox)row.FindControl("chkImagingServer");
                var isTftpServer = (CheckBox)row.FindControl("chkTftp");
                var isMulticast = (CheckBox)row.FindControl("chkMulticast");
                var isEms = (CheckBox)row.FindControl("chkEmServer");
                var dataKey = gvServers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

                foreach (var clusterServer in clusterServers)
                {
                    if (clusterServer.ComServerId == Convert.ToInt32(dataKey.Value))
                    {
                        cb.Checked = true;
                        role.Text = clusterServer.Role;
                        isImagingServer.Checked = clusterServer.IsImagingServer;
                        isTftpServer.Checked = clusterServer.IsTftpServer;
                        isMulticast.Checked = clusterServer.IsMulticastServer;

                        var comServer = Call.ClientComServerApi.Get(Convert.ToInt32(dataKey.Value));
                        if (comServer == null) continue;
                        if (!comServer.IsImagingServer)
                        {
                            isImagingServer.Checked = false;
                            isImagingServer.Enabled = false;
                            isImagingServer.Visible = false;
                        }

                        if (!comServer.IsTftpServer)
                        {
                            isTftpServer.Checked = false;
                            isTftpServer.Enabled = false;
                            isTftpServer.Visible = false;
                        }

                        if (!comServer.IsMulticastServer)
                        {
                            isMulticast.Checked = false;
                            isMulticast.Enabled = false;
                            isMulticast.Visible = false;
                        }
                        if (!comServer.IsEndpointManagementServer)
                        {
                            isEms.Checked = false;
                            isEms.Enabled = false;
                            isEms.Visible = false;
                        }

                    }
                }
            
            }
          
        }

        protected void buttonEdit_OnClick(object sender, EventArgs e)
        {

            ComServerCluster.Name = txtName.Text;
            ComServerCluster.Description = txtDescription.Text;
            ComServerCluster.IsDefault = chkDefault.Checked;

            var result = Call.ComServerClusterApi.Put(ComServerCluster.Id,ComServerCluster);
            if (result.Success)
            {
                var listOfServers = new List<EntityComServerClusterServer>();
                foreach (GridViewRow row in gvServers.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (!cb.Checked) continue;
                    var role = (DropDownList)row.FindControl("ddlRole");
                    var isImagingServer = (CheckBox)row.FindControl("chkImagingServer");
                    var isTftpServer = (CheckBox)row.FindControl("chkTftp");
                    var isMulticast = (CheckBox)row.FindControl("chkMulticast");
                    var isEms = (CheckBox)row.FindControl("chkEmServer");
                    var dataKey = gvServers.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;

                 

                    var clusterServer = new EntityComServerClusterServer();
                    clusterServer.ComServerClusterId = result.Id;
                    clusterServer.ComServerId = Convert.ToInt32(dataKey.Value);
                    clusterServer.Role = role.Text;
                    clusterServer.IsImagingServer = isImagingServer.Checked;
                    clusterServer.IsTftpServer = isTftpServer.Checked;
                    clusterServer.IsMulticastServer = isMulticast.Checked;
                    clusterServer.IsEndpointManagementServer = isEms.Checked;
                    listOfServers.Add(clusterServer);
                }

                var finalResult = Call.ComClusterServerApi.Post(listOfServers);
                if (finalResult.Success)
                    EndUserMessage = "Successfully Updated Cluster";
                else
                {
                    EndUserMessage = "Could Not Update Cluster.  Ensure That At Least One Server Is Selected";
                }
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
}