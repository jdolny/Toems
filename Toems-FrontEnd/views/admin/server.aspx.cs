using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.admin
{
    public partial class server : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (!IsPostBack)
            {
                PopulateForm();
            }

        }

        private void PopulateForm()
        {
            txtOrganization.Text = GetSetting(SettingStrings.CertificateOrganization);
            txtWebTimeout.Text = GetSetting(SettingStrings.WebUiTimeout);
            ddlComputerView.Text = GetSetting(SettingStrings.DefaultComputerView);
            ddlComputerSort.Text = GetSetting(SettingStrings.ComputerSortMode);
            ddlLoginPage.Text = GetSetting(SettingStrings.DefaultLoginPage);
            ddlImageReplication.Text = GetSetting(SettingStrings.DefaultImageReplication);
            ddlReplicationTime.Text = GetSetting(SettingStrings.ImageReplicationTime);
            BindComServers();
        }

        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOrganization.Text))
            {
                EndUserMessage = "Organization Must Not Be Empty";
                return;
            }
            if (!txtOrganization.Text.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                EndUserMessage = "Organization Can Only Contain Alphanumeric Characters And _ - space .";
                return;
            }
                var listSettings = new List<EntitySetting>
            {

                new EntitySetting
                {
                    Name = SettingStrings.CertificateOrganization,
                    Value = txtOrganization.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.CertificateOrganization).Id
                },

                new EntitySetting
                {
                    Name = SettingStrings.DefaultComputerView,
                    Value = ddlComputerView.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.DefaultComputerView).Id
                },
                 new EntitySetting
                {
                    Name = SettingStrings.ComputerSortMode,
                    Value = ddlComputerSort.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ComputerSortMode).Id
                },
                  new EntitySetting
                {
                    Name = SettingStrings.DefaultLoginPage,
                    Value = ddlLoginPage.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.DefaultLoginPage).Id
                },
                     new EntitySetting
                {
                    Name = SettingStrings.ImageReplicationTime,
                    Value = ddlReplicationTime.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ImageReplicationTime).Id
                },
                        new EntitySetting
                {
                    Name = SettingStrings.DefaultImageReplication,
                    Value = ddlImageReplication.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.DefaultImageReplication).Id
                },
                         new EntitySetting
                {
                    Name = SettingStrings.WebUiTimeout,
                    Value = txtWebTimeout.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.WebUiTimeout).Id
                },
            };


            if (Call.SettingApi.UpdateSettings(listSettings))
            {
                EndUserMessage = "Successfully Updated Settings";
            }
            else
            {
                EndUserMessage = "Could Not Update Settings";
            }

            if (ddlImageReplication.Text == "Selective")
            {
                var list = new List<EntityDefaultImageReplicationServer>();
                foreach (GridViewRow row in gvServers.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (cb == null || !cb.Checked) continue;
                    var dataKey = gvServers.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;
                    var imageReplicationComServer = new EntityDefaultImageReplicationServer();
                    imageReplicationComServer.ComServerId = Convert.ToInt32(dataKey.Value);


                    list.Add(imageReplicationComServer);
                }

                var z = Call.SettingApi.UpdateDefaultReplicationServers(list);
                if (!z.Success)
                {
                    EndUserMessage = z.ErrorMessage;
                    return;
                }
            }
        }

        private void BindComServers()
        {
            var replicationMode = (EnumImageReplication.ReplicationType)Enum.Parse(typeof(EnumImageReplication.ReplicationType),
                ddlImageReplication.SelectedValue);
            if (replicationMode != EnumImageReplication.ReplicationType.Selective)
                divComServers.Visible = false;
            else
            {
                divComServers.Visible = true;
                gvServers.DataSource = Call.ClientComServerApi.Get();
                gvServers.DataBind();

                var imageReplicationServers = Call.SettingApi.GetDefaultImageReplicationComServers();
                var entityImageReplicationServers = imageReplicationServers as EntityDefaultImageReplicationServer[] ?? imageReplicationServers.ToArray();
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

        protected void ddlImageReplication_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindComServers();
        }
    }
}