using System.DirectoryServices;
using Toems_Common;
using Toems_Common.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class ProvisionCompleteTasks(ServiceContext ctx)
    {
        private List<EntityGroupMembership> _groupMemberships = new();
        
        public void Run(EntityComputer computer)
        {
            AddComputerToOu(computer);
            AddComputerToToemsGroup(computer);
        }

        private void AddComputerToToemsGroup(EntityComputer computer)
        {
            if (string.IsNullOrEmpty(ctx.Setting.GetSettingValue(SettingStrings.NewProvisionDefaultGroup)))
            {
                ctx.Log.Debug("New Provision default group is not enabled.  Skipping");
                return;
            }

            var group = ctx.Group.GetGroupByName(ctx.Setting.GetSettingValue(SettingStrings.NewProvisionDefaultGroup));
            if (group == null) return;

            if (group.Type.Equals("Dynamic")) return;

            var groupMembership = new EntityGroupMembership();
            groupMembership.ComputerId = computer.Id;
            groupMembership.GroupId = group.Id;
            _groupMemberships.Add(groupMembership);
            ctx.GroupMembership.AddMembership(_groupMemberships);

        }

        private void AddComputerToOu(EntityComputer computer)
        {
            try
            {
                if (ctx.Setting.GetSettingValue(SettingStrings.NewProvisionAdCheck) != "1")
                {
                    ctx.Log.Debug("New Provision Active Directory check is not enabled.  Skipping");
                    return;
                }
                if (ctx.Setting.GetSettingValue(SettingStrings.LdapEnabled) != "1")
                {
                    ctx.Log.Debug("LDAP integration is not enabled.  Skipping");
                    return;
                }
                if (string.IsNullOrEmpty(ctx.Setting.GetSettingValue(SettingStrings.LdapServer)))
                {
                    ctx.Log.Debug("LDAP values not populated.  Skipping");
                    return;
                }

                var basePath = "LDAP://" + ctx.Setting.GetSettingValue(SettingStrings.LdapServer) + ":" +
                      ctx.Setting.GetSettingValue(SettingStrings.LdapPort) + "/";
                var username = ctx.Setting.GetSettingValue(SettingStrings.LdapBindUsername);
                var password =
                    ctx.Encryption.DecryptText(ctx.Setting.GetSettingValue(SettingStrings.LdapBindPassword));
                var baseDn = ctx.Setting.GetSettingValue(SettingStrings.LdapBaseDN);

                var entry = new DirectoryEntry(basePath + baseDn, username, password);
                var ldapAuth = ctx.Setting.GetSettingValue(SettingStrings.LdapAuthType);
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
                ctx.GroupMembership.AddMembership(_groupMemberships);

                return;
            }
            catch (Exception ex)
            {
                ctx.Log.Debug("Active Directory Bind Failed.");
                ctx.Log.Error(ex.Message);
                return;
            }
        }

        private void CreateMembershipList(EntityComputer computer, string dn, string baseDn)
        {
            if (dn == baseDn)
                return;
            var parentOu = dn.Substring(dn.IndexOf(",", StringComparison.Ordinal) + 1);
            var parentOuGroup = ctx.Group.GetGroupByDn(parentOu);
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
