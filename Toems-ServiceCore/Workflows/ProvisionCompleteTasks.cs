using log4net;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{
    public class ProvisionCompleteTasks(InfrastructureContext ictx, ServiceComputer serviceComputer, GroupService groupService, ServiceGroupMembership serviceGroupMembership)
    {
        private List<EntityGroupMembership> _groupMemberships = new();
        
        public void Run(EntityComputer computer)
        {
            AddComputerToOu(computer);
            AddComputerToToemsGroup(computer);
        }

        private void AddComputerToToemsGroup(EntityComputer computer)
        {
            if (string.IsNullOrEmpty(ictx.Settings.GetSettingValue(SettingStrings.NewProvisionDefaultGroup)))
            {
                ictx.Log.Debug("New Provision default group is not enabled.  Skipping");
                return;
            }

            var group = groupService.GetGroupByName(ictx.Settings.GetSettingValue(SettingStrings.NewProvisionDefaultGroup));
            if (group == null) return;

            if (group.Type.Equals("Dynamic")) return;

            var groupMembership = new EntityGroupMembership();
            groupMembership.ComputerId = computer.Id;
            groupMembership.GroupId = group.Id;
            _groupMemberships.Add(groupMembership);
            serviceGroupMembership.AddMembership(_groupMemberships);

        }

        private void AddComputerToOu(EntityComputer computer)
        {
            try
            {
                if (ictx.Settings.GetSettingValue(SettingStrings.NewProvisionAdCheck) != "1")
                {
                    ictx.Log.Debug("New Provision Active Directory check is not enabled.  Skipping");
                    return;
                }
                if (ictx.Settings.GetSettingValue(SettingStrings.LdapEnabled) != "1")
                {
                    ictx.Log.Debug("LDAP integration is not enabled.  Skipping");
                    return;
                }
                if (string.IsNullOrEmpty(ictx.Settings.GetSettingValue(SettingStrings.LdapServer)))
                {
                    ictx.Log.Debug("LDAP values not populated.  Skipping");
                    return;
                }

                var basePath = "LDAP://" + ictx.Settings.GetSettingValue(SettingStrings.LdapServer) + ":" +
                      ictx.Settings.GetSettingValue(SettingStrings.LdapPort) + "/";
                var username = ictx.Settings.GetSettingValue(SettingStrings.LdapBindUsername);
                var password =
                    ictx.Encryption.DecryptText(ictx.Settings.GetSettingValue(SettingStrings.LdapBindPassword));
                var baseDn = ictx.Settings.GetSettingValue(SettingStrings.LdapBaseDN);

                var entry = new DirectoryEntry(basePath + baseDn, username, password);
                var ldapAuth = ictx.Settings.GetSettingValue(SettingStrings.LdapAuthType);
                if (ldapAuth == "Basic")
                    entry.AuthenticationType = AuthenticationTypes.None;
                else if (ldapAuth == "Secure")
                    entry.AuthenticationType = AuthenticationTypes.Secure;

                var searcher = new DirectorySearcher(entry);
                searcher.Filter = "(&(cn=" + computer.Name + ")(objectcategory=Computer))";
                searcher.PropertiesToLoad.Add("distinguishedName");
                searcher.SizeLimit = 0;
                searcher.PageSize = 500;
                var result = searcher.FindOne();
                if (result == null)
                    return;
                var computerDn = (string)result.Properties["distinguishedName"][0];

                CreateMembershipList(computer, computerDn, baseDn);
                serviceGroupMembership.AddMembership(_groupMemberships);

                return;
            }
            catch (Exception ex)
            {
                ictx.Log.Debug("Active Directory Bind Failed.");
                ictx.Log.Error(ex.Message);
                return;
            }
        }

        private void CreateMembershipList(EntityComputer computer, string dn, string baseDn)
        {
            if (dn == baseDn)
                return;
            var parentOu = dn.Substring(dn.IndexOf(",", StringComparison.Ordinal) + 1);
            var parentOuGroup = groupService.GetGroupByDn(parentOu);
            if (parentOuGroup == null) return;
            if (parentOuGroup.Id < 1) return;
            var groupMembership = new EntityGroupMembership();
            groupMembership.ComputerId = computer.Id;
            groupMembership.GroupId = parentOuGroup.Id;
            _groupMemberships.Add(groupMembership);
            CreateMembershipList(computer, parentOu, baseDn);
        }
    }
}
