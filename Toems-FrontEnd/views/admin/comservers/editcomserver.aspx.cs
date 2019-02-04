using System;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class editcomserver : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
           
            ComServer.DisplayName = txtName.Text;
            ComServer.Url = txtUrl.Text;
            ComServer.Description = txtDescription.Text;
            ComServer.ReplicateStorage = chkReplicateStorage.Checked;
            var result = Call.ClientComServerApi.Put(ComServer.Id, ComServer);
            EndUserMessage = result.Success ? "Successfully Updated Server" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            txtName.Text = ComServer.DisplayName;
            txtUrl.Text = ComServer.Url;
            txtDescription.Text = ComServer.Description;
            chkReplicateStorage.Checked = ComServer.ReplicateStorage;
        }
    }
}