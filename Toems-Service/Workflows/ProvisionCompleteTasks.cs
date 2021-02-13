using log4net;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class ProvisionCompleteTasks
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ServiceComputer _computerService;
        private readonly ServiceGroup _groupService;
        private readonly ServiceGroupMembership _groupMembershipService;
        private List<EntityGroupMembership> _groupMemberships;

        public ProvisionCompleteTasks()
        {
            _groupService = new ServiceGroup();
            _computerService = new ServiceComputer();
            _groupMembershipService = new ServiceGroupMembership();
            _groupMemberships = new List<EntityGroupMembership>();
        }

        public void Run(EntityComputer computer)
        {
            AddComputerToOu(computer);
            AddComputerToToemsGroup(computer);
        }

        private void AddComputerToToemsGroup(EntityComputer computer)
        {
            if (string.IsNullOrEmpty(ServiceSetting.GetSettingValue(SettingStrings.NewProvisionDefaultGroup)))
            {
                Logger.Debug("New Provision default group is not enabled.  Skipping");
                return;
            }

            var group = _groupService.GetGroupByName(ServiceSetting.GetSettingValue(SettingStrings.NewProvisionDefaultGroup));
            if (group == null) return;

            if (group.Type.Equals("Dynamic")) return;

            var groupMembership = new EntityGroupMembership();
            groupMembership.ComputerId = computer.Id;
            groupMembership.GroupId = group.Id;
            _groupMemberships.Add(groupMembership);
            _groupMembershipService.AddMembership(_groupMemberships);

        }

        private void AddComputerToOu(EntityComputer computer)
        {
            try
            {
                if (ServiceSetting.GetSettingValue(SettingStrings.NewProvisionAdCheck) != "1")
                {
                    Logger.Debug("New Provision Active Directory check is not enabled.  Skipping");
                    return;
                }
                if (ServiceSetting.GetSettingValue(SettingStrings.LdapEnabled) != "1")
                {
                    Logger.Debug("LDAP integration is not enabled.  Skipping");
                    return;
                }
                if (string.IsNullOrEmpty(ServiceSetting.GetSettingValue(SettingStrings.LdapServer)))
                {
                    Logger.Debug("LDAP values not populated.  Skipping");
                    return;
                }

                var basePath = "LDAP://" + ServiceSetting.GetSettingValue(SettingStrings.LdapServer) + ":" +
                      ServiceSetting.GetSettingValue(SettingStrings.LdapPort) + "/";
                var username = ServiceSetting.GetSettingValue(SettingStrings.LdapBindUsername);
                var password =
                    new EncryptionServices().DecryptText(ServiceSetting.GetSettingValue(SettingStrings.LdapBindPassword));
                var baseDn = ServiceSetting.GetSettingValue(SettingStrings.LdapBaseDN);

                var entry = new DirectoryEntry(basePath + baseDn, username, password);
                var ldapAuth = ServiceSetting.GetSettingValue(SettingStrings.LdapAuthType);
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
                _groupMembershipService.AddMembership(_groupMemberships);

                return;
            }
            catch (Exception ex)
            {
                Logger.Debug("Active Directory Bind Failed.");
                Logger.Error(ex.Message);
                return;
            }
        }

        private void CreateMembershipList(EntityComputer computer, string dn, string baseDn)
        {
            if (dn == baseDn)
                return;
            var parentOu = dn.Substring(dn.IndexOf(",", StringComparison.Ordinal) + 1);
            var parentOuGroup = _groupService.GetGroupByDn(parentOu);
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
