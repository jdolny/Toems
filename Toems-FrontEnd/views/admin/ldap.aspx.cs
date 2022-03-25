using System;
using System.Collections.Generic;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin
{
    public partial class ldap : BasePages.Admin
    {
        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {

            var listSettings = new List<EntitySetting>
            {


                new EntitySetting
                {
                    Name = "Ldap Server",
                    Value = txtldapServer.Text,
                    Id = Call.SettingApi.GetSetting("Ldap Server").Id
                },
                new EntitySetting
                {
                    Name = "Ldap Port",
                    Value = txtldapPort.Text,
                    Id = Call.SettingApi.GetSetting("Ldap Port").Id
                },
                new EntitySetting
                {
                    Name = "Ldap Auth Attribute",
                    Value = txtldapAuthAttribute.Text,
                    Id = Call.SettingApi.GetSetting("Ldap Auth Attribute").Id
                },
                new EntitySetting
                {
                    Name = "Ldap Base DN",
                    Value = txtldapbasedn.Text,
                    Id = Call.SettingApi.GetSetting("Ldap Base DN").Id
                },
                new EntitySetting
                {
                    Name = "Ldap Auth Type",
                    Value = ddlldapAuthType.Text,
                    Id = Call.SettingApi.GetSetting("Ldap Auth Type").Id
                },
                new EntitySetting
                {
                    Name = "Ldap Group Filter",
                    Value = txtldapgroupfilter.Text,
                    Id = Call.SettingApi.GetSetting("Ldap Group Filter").Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.LdapBindUsername,
                    Value = txtLdapUsername.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.LdapBindUsername).Id
                }
            };

            if (!string.IsNullOrEmpty(txtLdapPassword.Text))
            {
                listSettings.Add(new EntitySetting()
                {
                    Name = SettingStrings.LdapBindPassword,
                    Value = txtLdapPassword.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.LdapBindPassword).Id
                });
            }




            EndUserMessage = Call.SettingApi.UpdateSettings(listSettings)
                ? "Successfully Updated Settings"
                : "Could Not Update Settings";


        }




        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (IsPostBack) return;

            var ldapEnabled = GetSetting(SettingStrings.LdapEnabled) == "1";
            if (!ldapEnabled)
            {
                txtLdap.Text = "LDAP Must First Be Enabled In The Security Settings";

            }
            else
                ad.Visible = true;
            txtldapServer.Text = GetSetting(SettingStrings.LdapServer);
            txtldapPort.Text = GetSetting(SettingStrings.LdapPort);
            txtldapAuthAttribute.Text = GetSetting(SettingStrings.LdapAuthAttribute);
            txtldapbasedn.Text = GetSetting(SettingStrings.LdapBaseDN);
            ddlldapAuthType.Text = GetSetting(SettingStrings.LdapAuthType);
            txtldapgroupfilter.Text = GetSetting(SettingStrings.LdapGroupFilter);
            txtLdapUsername.Text = GetSetting(SettingStrings.LdapBindUsername);

        }

        protected void btnTestBind_Click(object sender, EventArgs e)
        {
            var result = Call.SettingApi.TestADBind();
            if (result)
                EndUserMessage = "Test Was Successful";
            else
                EndUserMessage = "Bind Failed.  Check The Exception Log For More Info.";
        }
    }
}
