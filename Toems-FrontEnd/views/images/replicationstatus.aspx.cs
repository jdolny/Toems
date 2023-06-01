using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.images
{
    public partial class replicationstatus : BasePages.Images
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var remote = Call.SettingApi.IsStorageRemote();
                if (remote)
                    BindGrid();
                else
                    lblLocal.Text = "Replication Is Not Used With Local Storage";
            }
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            ImageEntity.ReplicationMode = (EnumImageReplication.ReplicationType)Enum.Parse(typeof(EnumImageReplication.ReplicationType), ddlReplicationMode.SelectedValue);

            var result = Call.ImageApi.Put(ImageEntity.Id, ImageEntity);
            EndUserMessage = result.Success ? "Successfully Updated Image" : result.ErrorMessage;


            if (ImageEntity.ReplicationMode == EnumImageReplication.ReplicationType.Selective)
            {
                var list = new List<EntityImageReplicationServer>();
                foreach (GridViewRow row in gvServers.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (cb == null || !cb.Checked) continue;
                    var dataKey = gvServers.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;
                    var imageReplicationComServer = new EntityImageReplicationServer();
                    imageReplicationComServer.ComServerId = Convert.ToInt32(dataKey.Value);
                    imageReplicationComServer.ImageId = ImageEntity.Id;

                    list.Add(imageReplicationComServer);
                }

                var z = Call.ImageApi.UpdateReplicationServers(list);
                if (!z.Success)
                {
                    EndUserMessage = z.ErrorMessage;
                    return;
                }
            }
        }

        private void BindGrid()
        {
            ddlReplicationMode.DataSource = Enum.GetNames(typeof(EnumImageReplication.ReplicationType));
            ddlReplicationMode.DataBind();

            ddlReplicationMode.SelectedValue = ImageEntity.ReplicationMode.ToString();

            gvCom.DataSource = Call.ImageApi.GetReplicationStatus(ImageEntity.Id);
            gvCom.DataBind();

            BindComServers();
        }

        private void BindComServers()
        {
            var replicationMode = (EnumImageReplication.ReplicationType)Enum.Parse(typeof(EnumImageReplication.ReplicationType),
                ddlReplicationMode.SelectedValue);
            if (replicationMode != EnumImageReplication.ReplicationType.Selective)
                divComServers.Visible = false;
            else
            {
                divComServers.Visible = true;
                gvServers.DataSource = Call.ClientComServerApi.Get();
                gvServers.DataBind();

                var imageReplicationServers = Call.ImageApi.GetImageReplicationComServers(ImageEntity.Id);
                var entityImageReplicationServers = imageReplicationServers as EntityImageReplicationServer[] ?? imageReplicationServers.ToArray();
                if (entityImageReplicationServers.Any())
                {
                    foreach (GridViewRow row in gvServers.Rows)
                    {
                        var cb = (CheckBox)row.FindControl("chkSelector");
                        var dataKey = gvServers.DataKeys[row.RowIndex];
                        if (dataKey == null) continue;

                        foreach (var comServer in entityImageReplicationServers)
                        {
                            if (comServer.ComServerId == Convert.ToInt32(dataKey.Value))
                            {
                                cb.Checked = true;
                            }
                        }

                    }
                }
            }

        }

        protected void ddlReplicationMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindComServers();
        }
    }
}