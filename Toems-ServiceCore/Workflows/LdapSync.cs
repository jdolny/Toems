using System.DirectoryServices;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class LdapSync(ServiceContext ctx)
    {
        private string _basePath;
        private string _username;
        private string _password;
        private List<EntityGroupMembership> _groupMemberships = new();
        private string _baseDn;
        private string _syncOU;
        
        public bool TestBind()
        {
            try
            {
                _basePath = "LDAP://" + ctx.Setting.GetSettingValue(SettingStrings.LdapServer) + ":" +
                      ctx.Setting.GetSettingValue(SettingStrings.LdapPort) + "/";
                _username = ctx.Setting.GetSettingValue(SettingStrings.LdapBindUsername);
                _password =
                    ctx.Encryption.DecryptText(ctx.Setting.GetSettingValue(SettingStrings.LdapBindPassword));
                _baseDn = ctx.Setting.GetSettingValue(SettingStrings.LdapBaseDN);

                var entry = InitializeEntry();
                var searcher = new DirectorySearcher(entry);
                searcher.Filter = "(objectCategory=organizationalUnit)";
                searcher.PropertiesToLoad.Add("ou");
                searcher.PropertiesToLoad.Add("distinguishedName");
                searcher.SizeLimit = 0;
                searcher.PageSize = 500;
                searcher.FindOne();
                return true;
            }
            catch(Exception ex)
            {
                ctx.Log.Debug("Active Directory Bind Failed.");
                ctx.Log.Error(ex.Message);
                return false;
            }
        }

        public bool Run()
        {
            ctx.Log.Debug("Starting Active Directory Sync");
            if (ctx.Setting.GetSettingValue(SettingStrings.LdapEnabled) != "1")
            {
                ctx.Log.Debug("LDAP integration is not enabled.  Skipping");
                return true;
            }
            if (string.IsNullOrEmpty(ctx.Setting.GetSettingValue(SettingStrings.LdapServer)))
            {
                ctx.Log.Debug("LDAP values not populated.  Skipping");
                return true;
            }

            _basePath = "LDAP://" + ctx.Setting.GetSettingValue(SettingStrings.LdapServer) + ":" +
                        ctx.Setting.GetSettingValue(SettingStrings.LdapPort) + "/";
            _username = ctx.Setting.GetSettingValue(SettingStrings.LdapBindUsername);
            _password =
                ctx.Encryption.DecryptText(ctx.Setting.GetSettingValue(SettingStrings.LdapBindPassword));
            _syncOU = ctx.Setting.GetSettingValue(SettingStrings.LdapSyncOU);
            _baseDn =  _syncOU + "," + ctx.Setting.GetSettingValue(SettingStrings.LdapBaseDN);
            _baseDn = _baseDn.Trim(',');

           
            var ous = GetOUs();
            var parents = GetParentOU(ous);
            CreateOuGroups(ous, parents);
            SyncComputers();
            UpdateMemberships();
            GetSecurityGroups();

            ctx.Log.Debug("Finished Active Directory Sync");
            return true;
        }

        private DirectoryEntry InitializeEntry()
        {
            var entry = new DirectoryEntry(_basePath + _baseDn, _username, _password);
            var ldapAuth = ctx.Setting.GetSettingValue(SettingStrings.LdapAuthType);
            if (ldapAuth == "Basic")
                entry.AuthenticationType = AuthenticationTypes.None;
            else if (ldapAuth == "Secure")
                entry.AuthenticationType = AuthenticationTypes.Secure;
            //else if (ldapAuth == "SSL")
               // entry.AuthenticationType = AuthenticationTypes.SecureSocketsLayer;

            return entry;

        }

        private Dictionary<string, string> GetOUs()
        {
            ctx.Log.Debug("Enumerating Active Directory Organizational Units");
            var ouDict = new Dictionary<string, string>();
            //add the base dn
            ouDict.Add(_baseDn, _baseDn);
            using (DirectoryEntry entry = InitializeEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = "(objectCategory=organizationalUnit)";
                    searcher.PropertiesToLoad.Add("ou");
                    searcher.PropertiesToLoad.Add("distinguishedName");
                    searcher.SizeLimit = 0;
                    searcher.PageSize = 500;
                    foreach (SearchResult res in searcher.FindAll())
                    {

                        if (!ouDict.Keys.Contains((string)res.Properties["distinguishedName"][0]))
                            ouDict.Add((string)res.Properties["distinguishedName"][0], (string)res.Properties["ou"][0]);
                    }
                }
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = "(objectCategory=Container)";
                    searcher.PropertiesToLoad.Add("cn");
                    searcher.PropertiesToLoad.Add("distinguishedName");
                    searcher.SizeLimit = 0;
                    searcher.PageSize = 500;
                    foreach (SearchResult res in searcher.FindAll())
                    {
                        if (!ouDict.Keys.Contains((string)res.Properties["distinguishedName"][0]))
                            ouDict.Add((string)res.Properties["distinguishedName"][0], (string)res.Properties["cn"][0]);
                    }
                }
            }


            
                
            return ouDict;
        }

        private Dictionary<string, string> GetParentOU(Dictionary<string, string> ous)
        {
            var parentDict = new Dictionary<string, string>();
            foreach (var ou in ous.Keys)
            {
                var parent = ou.Equals(_baseDn)
                    ? "AD-ROOT-OU"
                    : ou.Substring(ou.IndexOf(",", StringComparison.Ordinal) + 1);
                parentDict.Add(ou, parent);
            }
            return parentDict;
        }

        private void CreateOuGroups(Dictionary<string, string> ous, Dictionary<string, string> parents)
        {
            ctx.Log.Debug("Creating Groups Based On OU's");
            var addList = new List<EntityGroup>();
            var updateList = new List<EntityGroup>();
            foreach (var p in parents)
            {
                var group = new EntityGroup();
                string item;
                ous.TryGetValue(p.Key, out item);
                if (item != null)
                    group.Name = item;
                else
                    continue;
                group.Dn = p.Key;
                group.IsOu = true;
                group.ParentOu = p.Value;
                group.Type = "Static";
                group.Description = "Imported from Active Directory";
                group.ClusterId = -1;
                addList.Add(group);
            }

            ctx.Group.AddGroupList(addList);

            var groups = ctx.Group.GetAllAdGroups();
            foreach (var group in groups)
            {
                var parent = ctx.Group.GetGroupParentId(group);
                if (parent != null)
                {
                    group.ParentId = parent.Id.ToString();
                    updateList.Add(group);
                }
             
            }

            ctx.Group.UpdateGroupList(updateList);
          
            //set the root
            var rootOu = ctx.Group.GetGroupByName(_baseDn);
            if (rootOu != null)
            {
                rootOu.ParentId = "0";
                rootOu.Description = "Imported from Active Directory";
                ctx.Uow.GroupRepository.Update(rootOu, rootOu.Id);
                ctx.Uow.Save();
            }
        }

        private void SyncComputers()
        {
            ctx.Log.Debug("Synchronizing Computers From Active Directory");
            var allCompDict = new Dictionary<string ,string>();

            //Get All ad enabled computer excluding servers
            var enabledCompDict = new Dictionary<string, string>();
            using (DirectoryEntry entry = InitializeEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = "(&(objectCategory=Computer)(!userAccountControl:1.2.840.113556.1.4.803:=2)(!operatingSystem=*server*))";
                    searcher.PropertiesToLoad.Add("cn");
                    searcher.PropertiesToLoad.Add("distinguishedName");
                    searcher.SizeLimit = 0;
                    searcher.PageSize = 500;
                    foreach (SearchResult res in searcher.FindAll())
                        enabledCompDict.Add((string) res.Properties["distinguishedName"][0],
                            ((string) res.Properties["cn"][0]).ToUpper());
                }
            }

            //Get All ad disabled computer excluding servers
            var disabledCompDict = new Dictionary<string, string>();
            using (DirectoryEntry entry = InitializeEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = "(&(objectCategory=Computer)(userAccountControl:1.2.840.113556.1.4.803:=2)(!operatingSystem=*server*))";
                    searcher.PropertiesToLoad.Add("cn");
                    searcher.PropertiesToLoad.Add("distinguishedName");
                    searcher.SizeLimit = 0;
                    searcher.PageSize = 500;
                    foreach (SearchResult res in searcher.FindAll())
                        disabledCompDict.Add((string)res.Properties["distinguishedName"][0],
                            ((string)res.Properties["cn"][0]).ToUpper());
                }
            }

            disabledCompDict.ToList().ForEach(x => allCompDict[x.Key] = x.Value);
            enabledCompDict.ToList().ForEach(x => allCompDict[x.Key] = x.Value);

       
            if (allCompDict.Count > 0)
            {
                var currentAdComputers = ctx.Computer.GetAllAdComputers();
                var toArchive = (from adComputer in currentAdComputers
                                let doesExist = allCompDict.FirstOrDefault(x => x.Value == adComputer.Name)
                                where doesExist.Value == null
                                select adComputer.Id).ToList();

                foreach (var compId in toArchive)
                    ctx.Computer.ArchiveComputer(compId);                 
            }

            foreach (var comp in enabledCompDict)
            {
                var existing = ctx.Computer.GetByName(comp.Value);
                if (existing == null)
                {
                    var computerEntity = new EntityComputer();
                    computerEntity.Name = comp.Value;
                    computerEntity.IsAdSync = true;
                    computerEntity.AdDisabled = false;
                    computerEntity.ProvisionStatus = EnumProvisionStatus.Status.PreProvisioned;
                    computerEntity.CertificateId = -1;
                    var addResult = ctx.Computer.AddComputer(computerEntity);
                    if (addResult == null) continue;
                    if (addResult.Success)
                        CreateMembershipList(computerEntity, comp.Key);
                }
                else
                {
                    if (!existing.IsAdSync)
                    {
                        existing.IsAdSync = true;
                        existing.AdDisabled = false;
                        ctx.Computer.UpdateComputer(existing);
                    }
                    else if (existing.IsAdSync && existing.AdDisabled)
                    {
                        existing.AdDisabled = false;
                        ctx.Computer.UpdateComputer(existing);
                    }
                    CreateMembershipList(existing, comp.Key);
                }
            }


            foreach (var comp in disabledCompDict)
            {
                var existing = ctx.Computer.GetByName(comp.Value);
                if (existing == null)
                {
                    var computerEntity = new EntityComputer();
                    computerEntity.Name = comp.Value;
                    computerEntity.IsAdSync = true;
                    computerEntity.AdDisabled = true;
                    computerEntity.ProvisionStatus = EnumProvisionStatus.Status.PreProvisioned;
                    computerEntity.CertificateId = -1;
                    var addResult = ctx.Computer.AddComputer(computerEntity);
                    if (addResult == null) continue;
                    if (addResult.Success)
                        CreateMembershipList(computerEntity, comp.Key);
                }
                else
                {
                    if (!existing.IsAdSync)
                    {
                        existing.IsAdSync = true;
                        existing.AdDisabled = true;
                        ctx.Computer.UpdateComputer(existing);
                    }
                    else if (existing.IsAdSync && !existing.AdDisabled)
                    {
                        existing.AdDisabled = true;
                        ctx.Computer.UpdateComputer(existing);
                    }
                    CreateMembershipList(existing, comp.Key);
                }
            }

          
        }

        private void CreateMembershipList(EntityComputer computer, string dn)
        {
            if (dn == _baseDn)
                return;
            var parentOu = dn.Substring(dn.IndexOf(",", StringComparison.Ordinal) + 1);
            var parentOuGroup = ctx.Group.GetGroupByDn(parentOu);
            if (parentOuGroup == null) return;
            if (parentOuGroup.Id < 1) return;
            var groupMembership = new EntityGroupMembership();
            groupMembership.ComputerId = computer.Id;
            groupMembership.GroupId = parentOuGroup.Id;
            _groupMemberships.Add(groupMembership);
            CreateMembershipList(computer,parentOu);
        }

        private void UpdateMemberships()
        {
            var ddComputers = ctx.Computer.GetAllAdComputers();

            foreach (var computer in ddComputers)
            {
                var computerAdGroups = ctx.Computer.GetComputerAdGroups(computer.Id);
                foreach (var adGroup in computerAdGroups)
                {
                    var membership =
                        _groupMemberships.FirstOrDefault(x => x.ComputerId == computer.Id && x.GroupId == adGroup.Id);
                    if (membership == null)
                        ctx.GroupMembership.DeleteByIds(computer.Id, adGroup.Id);

                }
            }
            ctx.GroupMembership.AddMembership(_groupMemberships);
        }

        private void GetSecurityGroups()
        {
            ctx.Log.Debug("Getting Active Directory Security Groups");
            var securityGroups = new Dictionary<string, string>();
            using (DirectoryEntry entry = InitializeEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = "(groupType:1.2.840.113556.1.4.803:=2147483648)";
                    searcher.PropertiesToLoad.Add("cn");
                    searcher.PropertiesToLoad.Add("distinguishedName");
                    searcher.SizeLimit = 0;
                    searcher.PageSize = 500;
                    foreach (SearchResult res in searcher.FindAll())
                        securityGroups.Add((string)res.Properties["distinguishedName"][0],
                            ((string)res.Properties["cn"][0]).ToUpper());
                }
            }

            var listMemberships = new List<EntityGroupMembership>();

            foreach (var securityGroup in securityGroups)
            {
                var securityGroupComputers = new List<string>();
                using (DirectoryEntry entry = InitializeEntry())
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = $"(&(objectCategory=Computer)(memberOf:1.2.840.113556.1.4.1941:={securityGroup.Key}))";
                        searcher.PropertiesToLoad.Add("cn");
                        searcher.SizeLimit = 0;
                        searcher.PageSize = 500;
                        try
                        {
                            foreach (SearchResult res in searcher.FindAll())
                                securityGroupComputers.Add(((string)res.Properties["cn"][0]));
                        }
                        catch {//continue
                              }
                    }
                }

                if (securityGroupComputers.Any())
                {
                    EntityGroup group = ctx.Group.GetGroupByDn(securityGroup.Key);
                    if (group == null)
                    {
                        group = new EntityGroup();
                        group.Name = securityGroup.Value;
                        group.Dn = securityGroup.Key;
                        group.IsOu = false;
                        group.IsSecurityGroup = true;
                        group.Type = "Static";
                        group.Description = "Security Group Imported from Active Directory";
                        group.ClusterId = -1;

                        ctx.Group.AddGroup(group);
                    }
                    foreach (var computer in securityGroupComputers)
                    {
                       
                        var entityComputer = ctx.Computer.GetByName(computer);
                        if (entityComputer == null) continue;

                        var groupMembership = new EntityGroupMembership();
                        groupMembership.ComputerId = entityComputer.Id;
                        groupMembership.GroupId = group.Id;
                        listMemberships.Add(groupMembership);
                    }
                }
            }

            var ddComputers = ctx.Computer.GetAllAdComputers();

            foreach (var computer in ddComputers)
            {
                var computerAdSecurityGroups = ctx.Computer.GetComputerAdSecurityGroups(computer.Id);
                foreach (var adSecurityGroup in computerAdSecurityGroups)
                {
                    var membership =
                        listMemberships.FirstOrDefault(x => x.ComputerId == computer.Id && x.GroupId == adSecurityGroup.Id);
                    if (membership == null)
                        ctx.GroupMembership.DeleteByIds(computer.Id, adSecurityGroup.Id);

                }
            }
            
            ctx.GroupMembership.AddMembership(listMemberships);

            foreach (var g in ctx.Group.GetAllAdSecurityGroups())
            {
                if (!ctx.Group.GetGroupMembers(g.Id).Any())
                {
                    g.IsHidden = true;
                    ctx.Group.UpdateGroup(g);
                }
            }
        }
    }
}
