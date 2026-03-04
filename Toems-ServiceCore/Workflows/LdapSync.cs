using log4net;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{
    public class LdapSync(InfrastructureContext ictx, ServiceComputer serviceComputer, GroupService groupService, ServiceGroupMembership serviceGroupMembership)
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
                _basePath = "LDAP://" + ictx.Settings.GetSettingValue(SettingStrings.LdapServer) + ":" +
                      ictx.Settings.GetSettingValue(SettingStrings.LdapPort) + "/";
                _username = ictx.Settings.GetSettingValue(SettingStrings.LdapBindUsername);
                _password =
                    ictx.Encryption.DecryptText(ictx.Settings.GetSettingValue(SettingStrings.LdapBindPassword));
                _baseDn = ictx.Settings.GetSettingValue(SettingStrings.LdapBaseDN);

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
                ictx.Log.Debug("Active Directory Bind Failed.");
                ictx.Log.Error(ex.Message);
                return false;
            }
        }

        public bool Run()
        {
            ictx.Log.Debug("Starting Active Directory Sync");
            if (ictx.Settings.GetSettingValue(SettingStrings.LdapEnabled) != "1")
            {
                ictx.Log.Debug("LDAP integration is not enabled.  Skipping");
                return true;
            }
            if (string.IsNullOrEmpty(ictx.Settings.GetSettingValue(SettingStrings.LdapServer)))
            {
                ictx.Log.Debug("LDAP values not populated.  Skipping");
                return true;
            }

            _basePath = "LDAP://" + ictx.Settings.GetSettingValue(SettingStrings.LdapServer) + ":" +
                        ictx.Settings.GetSettingValue(SettingStrings.LdapPort) + "/";
            _username = ictx.Settings.GetSettingValue(SettingStrings.LdapBindUsername);
            _password =
                ictx.Encryption.DecryptText(ictx.Settings.GetSettingValue(SettingStrings.LdapBindPassword));
            _syncOU = ictx.Settings.GetSettingValue(SettingStrings.LdapSyncOU);
            _baseDn =  _syncOU + "," + ictx.Settings.GetSettingValue(SettingStrings.LdapBaseDN);
            _baseDn = _baseDn.Trim(',');

           
            var ous = GetOUs();
            var parents = GetParentOU(ous);
            CreateOuGroups(ous, parents);
            SyncComputers();
            UpdateMemberships();
            GetSecurityGroups();

            ictx.Log.Debug("Finished Active Directory Sync");
            return true;
        }

        private DirectoryEntry InitializeEntry()
        {
            var entry = new DirectoryEntry(_basePath + _baseDn, _username, _password);
            var ldapAuth = ictx.Settings.GetSettingValue(SettingStrings.LdapAuthType);
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
            ictx.Log.Debug("Enumerating Active Directory Organizational Units");
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
            ictx.Log.Debug("Creating Groups Based On OU's");
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

            groupService.AddGroupList(addList);

            var groups = groupService.GetAllAdGroups();
            foreach (var group in groups)
            {
                var parent = groupService.GetGroupParentId(group);
                if (parent != null)
                {
                    group.ParentId = parent.Id.ToString();
                    updateList.Add(group);
                }
             
            }

            groupService.UpdateGroupList(updateList);
          
            //set the root
            var rootOu = groupService.GetGroupByName(_baseDn);
            if (rootOu != null)
            {
                rootOu.ParentId = "0";
                rootOu.Description = "Imported from Active Directory";
                var uow = new UnitOfWork();
                uow.GroupRepository.Update(rootOu, rootOu.Id);
                uow.Save();
            }
        }

        private void SyncComputers()
        {
            ictx.Log.Debug("Synchronizing Computers From Active Directory");
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
                var currentAdComputers = serviceComputer.GetAllAdComputers();
                var toArchive = (from adComputer in currentAdComputers
                                let doesExist = allCompDict.FirstOrDefault(x => x.Value == adComputer.Name)
                                where doesExist.Value == null
                                select adComputer.Id).ToList();

                foreach (var compId in toArchive)
                    serviceComputer.ArchiveComputer(compId);                 
            }

            foreach (var comp in enabledCompDict)
            {
                var existing = serviceComputer.GetByName(comp.Value);
                if (existing == null)
                {
                    var computerEntity = new EntityComputer();
                    computerEntity.Name = comp.Value;
                    computerEntity.IsAdSync = true;
                    computerEntity.AdDisabled = false;
                    computerEntity.ProvisionStatus = EnumProvisionStatus.Status.PreProvisioned;
                    computerEntity.CertificateId = -1;
                    var addResult = serviceComputer.AddComputer(computerEntity);
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
                        serviceComputer.UpdateComputer(existing);
                    }
                    else if (existing.IsAdSync && existing.AdDisabled)
                    {
                        existing.AdDisabled = false;
                        serviceComputer.UpdateComputer(existing);
                    }
                    CreateMembershipList(existing, comp.Key);
                }
            }


            foreach (var comp in disabledCompDict)
            {
                var existing = serviceComputer.GetByName(comp.Value);
                if (existing == null)
                {
                    var computerEntity = new EntityComputer();
                    computerEntity.Name = comp.Value;
                    computerEntity.IsAdSync = true;
                    computerEntity.AdDisabled = true;
                    computerEntity.ProvisionStatus = EnumProvisionStatus.Status.PreProvisioned;
                    computerEntity.CertificateId = -1;
                    var addResult = serviceComputer.AddComputer(computerEntity);
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
                        serviceComputer.UpdateComputer(existing);
                    }
                    else if (existing.IsAdSync && !existing.AdDisabled)
                    {
                        existing.AdDisabled = true;
                        serviceComputer.UpdateComputer(existing);
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
            var parentOuGroup = groupService.GetGroupByDn(parentOu);
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
            var ddComputers = serviceComputer.GetAllAdComputers();

            foreach (var computer in ddComputers)
            {
                var computerAdGroups = serviceComputer.GetComputerAdGroups(computer.Id);
                foreach (var adGroup in computerAdGroups)
                {
                    var membership =
                        _groupMemberships.FirstOrDefault(x => x.ComputerId == computer.Id && x.GroupId == adGroup.Id);
                    if (membership == null)
                        serviceGroupMembership.DeleteByIds(computer.Id, adGroup.Id);

                }
            }
            serviceGroupMembership.AddMembership(_groupMemberships);
        }

        private void GetSecurityGroups()
        {
            ictx.Log.Debug("Getting Active Directory Security Groups");
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
                    EntityGroup group = groupService.GetGroupByDn(securityGroup.Key);
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

                        groupService.AddGroup(group);
                    }
                    foreach (var computer in securityGroupComputers)
                    {
                       
                        var entityComputer = serviceComputer.GetByName(computer);
                        if (entityComputer == null) continue;

                        var groupMembership = new EntityGroupMembership();
                        groupMembership.ComputerId = entityComputer.Id;
                        groupMembership.GroupId = group.Id;
                        listMemberships.Add(groupMembership);
                    }
                }
            }

            var ddComputers = serviceComputer.GetAllAdComputers();

            foreach (var computer in ddComputers)
            {
                var computerAdSecurityGroups = serviceComputer.GetComputerAdSecurityGroups(computer.Id);
                foreach (var adSecurityGroup in computerAdSecurityGroups)
                {
                    var membership =
                        listMemberships.FirstOrDefault(x => x.ComputerId == computer.Id && x.GroupId == adSecurityGroup.Id);
                    if (membership == null)
                        serviceGroupMembership.DeleteByIds(computer.Id, adSecurityGroup.Id);

                }
            }
            
            serviceGroupMembership.AddMembership(listMemberships);

            foreach (var g in groupService.GetAllAdSecurityGroups())
            {
                if (!groupService.GetGroupMembers(g.Id).Any())
                {
                    g.IsHidden = true;
                    groupService.UpdateGroup(g);
                }
            }
        }
    }
}
