using System;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class comservers : BasePages.MasterBaseMaster
    {
        public EntityClientComServer ComServer { get; set; }
        public EntityComServerCluster Cluster { get; set; }
        private BasePages.Admin AdminBasePage { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            AdminBasePage = Page as BasePages.Admin;
            ComServer= AdminBasePage.ComServer;
            Cluster = AdminBasePage.ComServerCluster;

            if (ComServer == null && Cluster == null)
            {
                divLevel3.Visible = false;
                btnDelete.Visible = false;
                btnDeleteCluster.Visible = false;
            }
            else if(ComServer != null)
            {
                divLevel2.Visible = false;
                divComServer.Visible = true;
                divCluster.Visible = false;
                btnDelete.Visible = true;
            }
            else
            {
                divLevel2.Visible = false;
                divComServer.Visible = false;
                divCluster.Visible = true;
                btnDeleteCluster.Visible = true;
            }
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete This Com Server?";
            DisplayConfirm();
        }

        protected void btnDeleteCluster_OnClick(object sender, EventArgs e)
        {
            lblTitle.Text = "Delete This Cluster?";
            DisplayConfirm();
        }

        protected void OkButton_Click(object sender, EventArgs e)
        {
            if (ComServer != null)
            {
                var result = AdminBasePage.Call.ClientComServerApi.Delete(ComServer.Id);
                if (result.Success)
                {
                    PageBaseMaster.EndUserMessage = "Successfully Deleted Com Server";
                    Response.Redirect("~/views/admin/comservers/comservers.aspx?level=2");
                }
                else
                {
                    PageBaseMaster.EndUserMessage = result.ErrorMessage;
                }
            }
            else if (Cluster != null)
            {
                var result = AdminBasePage.Call.ComServerClusterApi.Delete(Cluster.Id);
                if (result.Success)
                {
                    PageBaseMaster.EndUserMessage = "Successfully Deleted Cluster";
                    Response.Redirect("~/views/admin/comservers/comservercluster.aspx?level=2");
                }
                else
                {
                    PageBaseMaster.EndUserMessage = result.ErrorMessage;
                }
            }
        }
    }
}