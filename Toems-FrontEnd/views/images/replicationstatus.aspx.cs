using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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

        private void BindGrid()
        {
            gvCom.DataSource = Call.ImageApi.GetReplicationStatus(ImageEntity.Id);
            gvCom.DataBind();
        }
    }
}