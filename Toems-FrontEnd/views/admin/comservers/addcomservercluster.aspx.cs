using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class addcomservercluster : BasePages.Admin
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
            gvServers.DataSource = Call.ClientComServerApi.Get();
            gvServers.DataBind();

            foreach (GridViewRow row in gvServers.Rows)
            {
                var isImagingServer = (CheckBox)row.FindControl("chkImagingServer");
                var isTftpServer = (CheckBox)row.FindControl("chkTftp");
                var isMulticast = (CheckBox)row.FindControl("chkMulticast");
                var isEms = (CheckBox)row.FindControl("chkEmServer");
                var dataKey = gvServers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

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

        protected void buttonAdd_OnClick(object sender, EventArgs e)
        {
            var comServerCluster = new EntityComServerCluster();
            comServerCluster.Name = txtName.Text;
            comServerCluster.Description = txtDescription.Text;
            comServerCluster.IsDefault = chkDefault.Checked;

            var counter = 0;
            foreach (GridViewRow row in gvServers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                if (!cb.Checked) continue;
                counter++;
            }

            if(counter == 0)
            {
                EndUserMessage = "A Com Server Cluster Must Have At Least 1 Server Selected";
                return;
            }

            var result = Call.ComServerClusterApi.Post(comServerCluster);
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
                {
                    EndUserMessage = "Successfully Added Cluster";
                    Response.Redirect("~/views/admin/comservers/editcomservercluster.aspx?level=2&clusterId=" + result.Id);
                }
                else
                {
                    EndUserMessage = "Could Not Add Cluster";
                }
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
}