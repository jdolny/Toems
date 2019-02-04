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
        }

        protected void buttonAdd_OnClick(object sender, EventArgs e)
        {
            var comServerCluster = new EntityComServerCluster();
            comServerCluster.Name = txtName.Text;
            comServerCluster.Description = txtDescription.Text;
            comServerCluster.IsDefault = chkDefault.Checked;

            var result = Call.ComServerClusterApi.Post(comServerCluster);
            if (result.Success)
            {
                var listOfServers = new List<EntityComServerClusterServer>();
                foreach (GridViewRow row in gvServers.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (!cb.Checked) continue;
                    var role = (DropDownList)row.FindControl("ddlRole");
                    var dataKey = gvServers.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;

                    var clusterServer = new EntityComServerClusterServer();
                    clusterServer.ComServerClusterId = result.Id;
                    clusterServer.ComServerId = Convert.ToInt32(dataKey.Value);
                    clusterServer.Role = role.Text;
                    listOfServers.Add(clusterServer);
                }

                if (listOfServers.Count == 0)
                {
                    EndUserMessage = "Successfully Added Cluster";
                    Response.Redirect("~/views/admin/comservers/editcomservercluster.aspx?level=2&clusterId=" + result.Id);
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