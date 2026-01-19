using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_Service.Workflows;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputer(EntityContext ectx, ServiceUser userService)
    {
        public List<EntityComputer> SearchComputers(DtoComputerFilter filter, int userId)
        {
            if(filter.Categories == null) filter.Categories = new List<string>();
            var categoryFilterIds = filter.Categories
                   .Select(catName => ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName)))
                   .Where(category => category != null)
                   .Select(category => category.Id)
                   .ToList();

            var list = ectx.Uow.ComputerRepository.SearchAllComputers(filter,userId,categoryFilterIds);
            
            
            
            foreach (var c in list)
            {
                var currentImage = GetEffectiveImage(c.Id);
                c.CurrentImage = currentImage?.Name;
            }


            var computerAcl = userService.GetAllowedComputers(userId);
            return computerAcl.ComputerManagementEnforced
                ? list.Where(c => computerAcl.AllowedComputerIds.Contains(c.Id)).ToList()
                : list.ToList();

        }

        public DtoActionResult RestoreComputer(int computerId)
        {
            var u = GetComputer(computerId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Computer Not Found", Id = 0 };
           u.ProvisionStatus = EnumProvisionStatus.Status.PreProvisioned;     
            u.ArchiveDateTime = null;
            u.Name = u.Name.Split('#').First();
            if (u.Name.Contains(":")) u.ProvisionStatus = EnumProvisionStatus.Status.ImageOnly;
            if(ectx.Uow.ComputerRepository.Exists(x => x.Name.Equals(u.Name)))
                return new DtoActionResult() { ErrorMessage = "Could Not Restore Computer.  A Computer With Name " + u.Name + " Already Exists"};
            ectx.Uow.ComputerRepository.Update(u, u.Id);
            ectx.Uow.Save();
            return new DtoActionResult() { Id = u.Id, Success = true };
        }

        public DtoActionResult ArchiveComputer(int computerId)
        {
            var u = GetComputer(computerId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Computer Not Found", Id = 0 };
            if (u.ProvisionStatus == EnumProvisionStatus.Status.Archived) return new DtoActionResult() { Id = u.Id, Success = true };
            u.ProvisionStatus = EnumProvisionStatus.Status.Archived;
            u.Name = u.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
            u.ArchiveDateTime = DateTime.Now;
            u.ImagingClientId = null;
            u.ImagingMac = null;
            ectx.Uow.ComputerRepository.Update(u,u.Id);
            ectx.Uow.CertificateRepository.DeleteRange(x => x.Id == u.CertificateId);
            ectx.Uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == u.Id);
            ectx.Uow.NicInventoryRepository.DeleteRange(x => x.ComputerId == u.Id);
            u.CertificateId = -1;
            ectx.Uow.Save();
            return new DtoActionResult() {Id = u.Id, Success = true};
        }

        public DtoActionResult ArchiveComputerKeepGroups(int computerId)
        {
            var u = GetComputer(computerId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Computer Not Found", Id = 0 };
            if (u.ProvisionStatus == EnumProvisionStatus.Status.Archived) return new DtoActionResult() { Id = u.Id, Success = true };
            u.ProvisionStatus = EnumProvisionStatus.Status.Archived;
            u.Name = u.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
            u.ArchiveDateTime = DateTime.Now;
            ectx.Uow.ComputerRepository.Update(u, u.Id);
            ectx.Uow.CertificateRepository.DeleteRange(x => x.Id == u.CertificateId);
            u.CertificateId = -1;
            ectx.Uow.Save();
            return new DtoActionResult() { Id = u.Id, Success = true };
        }

        public DtoActionResult AddComputer(EntityComputer computer)
        {
            computer.Name = computer.Name.ToUpper();
            var validationResult = ValidateComputer(computer, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.ComputerRepository.Insert(computer);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = computer.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult ClearImagingClientId(int computerId)
        {
            var u = GetComputer(computerId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Computer Not Found", Id = 0 };
            u.ImagingClientId = string.Empty;
            u.ImagingMac = string.Empty;
            ectx.Uow.ComputerRepository.Update(u, u.Id);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public DtoActionResult DeleteComputer(int computerId)
        {
            var u = GetComputer(computerId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Computer Not Found", Id = 0};
            ectx.Uow.ComputerRepository.Delete(computerId);
            ectx.Uow.ComputerLogRepository.DeleteRange(x => x.ComputerId == computerId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public List<EntityGroup> GetComputerGroups(int computerId)
        {
            return ectx.Uow.ComputerRepository.GetAllComputerGroups(computerId);
        }

        public List<DtoGroupImage> GetComputerGroupsWithImage(int computerId)
        {
            return ectx.Uow.ComputerRepository.GetAllComputerGroupsWithImage(computerId);
        }

        public List<EntityPolicy> GetComputerPolicies(int computerId)
        {
            return ectx.Uow.ComputerRepository.GetComputerPolicies(computerId);
        }

        public List<DtoComputerPolicyHistory> GetPolicyHistory(int computerId)
        {
            return ectx.Uow.ComputerRepository.GetPolicyHistory(computerId);
        }

        public List<DtoModule> GetComputerModules(int computerId)
        {
            return ectx.Uow.ComputerRepository.GetComputerModules(computerId);
        }

        public List<EntityWingetModule> GetComputerWingetUpgrades(string clientGuid)
        {
            var client = ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == clientGuid);
            return ectx.Uow.ComputerRepository.GetComputerWingetUpdateModules(client.Id);
        }

        public EntityWinPeModule GetEffectiveWinPeModule(int computerId)
        {
            var computer = GetComputer(computerId);
            var winPeModule = new ServiceWinPeModule().GetModule(computer.WinPeModuleId);
            if (winPeModule != null) return winPeModule;

            //check for an image profile via group since computer doesn't have image directly assigned
            var computerGroups = ectx.Uow.ComputerRepository.GetAllComputerGroups(computerId).OrderBy(x => x.ImagingPriority).ThenBy(x => x.Name).ToList();

            if (computerGroups.Count == 0)
            {
                return null;
            }
            else
            {
                foreach (var group in computerGroups)
                {
                    winPeModule = new ServiceWinPeModule().GetModule(group.WinPeModuleId);
                    if (winPeModule != null) return winPeModule;
                }

                //no images assigned to any groups
                return null;
            }

        }

        public ImageProfileWithImage GetEffectiveImage(int computerId)
        {
            var computer = GetComputer(computerId);
            var imageProfile = new ServiceImageProfile().ReadProfile(computer.ImageProfileId);
            if (imageProfile != null) return imageProfile;
            
            var computerGroups = ectx.Uow.ComputerRepository.GetAllComputerGroups(computerId).OrderBy(x => x.ImagingPriority).ThenBy(x => x.Name).ToList();

            if (computerGroups.Count == 0)
            {
                return null;
            }
            else
            {
                foreach (var group in computerGroups)
                {
                    imageProfile = new ServiceImageProfile().ReadProfile(group.ImageProfileId);
                    if (imageProfile != null) return imageProfile;
                }

                //no images assigned to any groups
                return null;  
            }
        }

        public string GetEffectivePolicy(int computerId, EnumPolicy.Trigger trigger, string comServerUrl)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(computerId);
            if (computer == null) return string.Empty;

            var policyRequest = new DtoPolicyRequest();
            policyRequest.Trigger = trigger;
            policyRequest.CurrentComServer = comServerUrl;
            policyRequest.ClientIdentity = new DtoClientIdentity();
            policyRequest.ClientIdentity.Guid = computer.Guid;
            policyRequest.ClientIdentity.Name = computer.Name;

            var policy = new Toems_Service.Workflows.GetClientPolicies().Execute(policyRequest,computerId);
            return JsonConvert.SerializeObject(policy.Policies, Formatting.Indented);
        }

        public EntityComputer GetComputer(int computerId)
        {
            return ectx.Uow.ComputerRepository.GetById(computerId);
        }

        public List<EntityGroup> GetComputerAdGroups(int computerId)
        {
           return ectx.Uow.ComputerRepository.GetComputerAdGroups(computerId);
        }

        public List<EntityGroup> GetComputerAdSecurityGroups(int computerId)
        {
            return ectx.Uow.ComputerRepository.GetComputerAdSecurityGroups(computerId);
        }

        public List<EntityClientComServer> GetEmServers(int computerId)
        {
            var list = new List<EntityClientComServer>();
            var computer = GetComputer(computerId);
            var result = new Toems_Service.Workflows.GetCompEmServers().Run(computer.Guid);
            foreach(var r in result)
            {
                list.Add(ectx.Uow.ClientComServerRepository.GetById(r.ComServerId));
            }
            return list;
        }

        public List<EntityClientComServer> GetTftpServers(int computerId)
        {
            return new Toems_Service.Workflows.GetCompTftpServers().Run(computerId);
        }

        public List<EntityClientComServer> GetImageServers(int computerId)
        {
            return new Toems_Service.Workflows.GetCompImagingServers().Run(computerId,true);
        }

        public EntityComputer GetByInstallationId(string installationid)
        {
            return ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.InstallationId == installationid);
        }

        public EntityComputer GetByName(string computerName)
        {
            return ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Name == computerName && x.ProvisionStatus != EnumProvisionStatus.Status.Archived);
        }

        public EntityComputer GetByNameForReset(string computerName)
        {
            return ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Name == computerName);
        }


        public EntityComputer GetByGuid(string computerGuid)
        {
            return ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == computerGuid);
        }

      

        public List<EntityComputerCategory> GetComputerCategories(int computerId)
        {
            return ectx.Uow.ComputerCategoryRepository.Get(x => x.ComputerId == computerId);
        }

        public List<EntityCustomComputerAttribute> GetCustomAttributes(int computerId)
        {
            return ectx.Uow.CustomComputerAttributeRepository.Get(x => x.ComputerId == computerId);
        }


       

        public List<EntityComputer> GetArchived(DtoSearchFilterCategories filter)
        {
            return ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)) && s.ProvisionStatus == EnumProvisionStatus.Status.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityComputer> GetAllAdComputers()
        {
            return ectx.Uow.ComputerRepository.Get(s => s.IsAdSync);
        }

        public List<EntityComputer> SearchForGroup(DtoSearchFilter filter)
        {
            return ectx.Uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) && (s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned || s.ProvisionStatus == EnumProvisionStatus.Status.Provisioned || s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly)).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityComputer> SearchPreProvision(DtoSearchFilter filter)
        {
            return ectx.Uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) && s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntitySoftwareInventory> GetComputerSoftware(int id, string searchString = "")
        {
            return ectx.Uow.ComputerRepository.GetComputerSoftware(id, searchString);
          
        }

        public List<EntityCertificateInventory> GetComputerCertificates(int id, string searchString = "")
        {
            return ectx.Uow.ComputerRepository.GetComputerCertificates(id, searchString);

        }

        public DtoActionResult UpdateSocketResult(string result, string clientIdentity)
        {
            var client = ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == clientIdentity);
            if (client == null) return new DtoActionResult() { ErrorMessage = "Client Not Found", Success = false };
            client.LastSocketResult = result;
            ectx.Uow.ComputerRepository.Update(client, client.Id);
            ectx.Uow.Save();
            return new DtoActionResult() { Success = true, Id = client.Id };
        }

        public bool SendMessage(int id, DtoMessage message)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Message";
                socketRequest.message = JsonConvert.SerializeObject(message);
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool StartRemoteControl(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Start_Remote_Control";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool GetSystemUptime(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "System_Uptime";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool RunModule(int computerId, string moduleGuid)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(computerId);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;

            var module = new ServiceModule().GetModuleIdFromGuid(moduleGuid);
            if (module == null) return false;
            var clientPolicy = new ClientPolicyJson().CreateInstantModule(module);

            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Run_Module";
                socketRequest.message = JsonConvert.SerializeObject(clientPolicy);
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }


        public bool ForceCheckin(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Force_Checkin";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool CollectInventory(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if(socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Collect_Inventory";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey,socketRequest);
            }

            return true;
        }

        public bool GetLoggedInUsers(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Current_Users";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool GetStatus(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Get_Status";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;

        }

        public bool GetServiceLog(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Logs";
                socketRequest.message = "Service.log";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;

        }

        public bool Reboot(int id)
        {
            return new Toems_Service.Workflows.PowerManagement().RebootComputer(id);
        }

        public bool Shutdown(int id)
        {
            return new Toems_Service.Workflows.PowerManagement().ShutdownComputer(id);
        }

        public bool Wakeup(int id)
        {
            new Toems_Service.Workflows.PowerManagement().WakeupComputer(id);
            return true;
        }

        public List<DtoComputerUpdates> GetUpdates(int id, string searchString = "")
        {
            return ectx.Uow.ComputerRepository.GetWindowsUpdates(id, searchString);

        }

        public List<EntityUserLogin> GetUserLogins(int id, string searchString = "")
        {
            return ectx.Uow.UserLoginRepository.Get(x => x.ComputerId == id && x.UserName.Contains(searchString)).OrderByDescending(x => x.LoginDateTime).ToList();
        }

        public List<DtoCustomComputerInventory> GetCustomInventory(int id)
        {
            return ectx.Uow.ComputerRepository.GetCustomComputerInventory(id);
        }

        public string AllCount()
        {
            return ectx.Uow.ComputerRepository.Count();
        }

        public string TotalCount()
        {
            return ectx.Uow.ComputerRepository.Count(s => s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned || s.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
        }

        public string TotalActiveCount()
        {
            return ectx.Uow.ComputerRepository.Count(s => s.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned && s.ProvisionStatus != EnumProvisionStatus.Status.Archived && s.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved && s.ProvisionStatus != EnumProvisionStatus.Status.ImageOnly);
        }

        public string ClearLastSocketResult(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return string.Empty;
            computer.LastSocketResult = string.Empty;
            ectx.Uow.ComputerRepository.Update(computer, computer.Id);
            ectx.Uow.Save();
            return computer.LastSocketResult;
        }

        public string LastSocketResult(int id)
        {
            var computer = ectx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return string.Empty;
            return computer.LastSocketResult;
        }

        public string ArchivedCount()
        {
            return ectx.Uow.ComputerRepository.Count(s => s.ProvisionStatus == EnumProvisionStatus.Status.Archived);
        }

        public string ImageOnlyCount()
        {
            return ectx.Uow.ComputerRepository.Count(s => s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly);
        }

        public string TotalPreProvisionCount()
        {
            return ectx.Uow.ComputerRepository.Count(x => x.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned);
        }


        public DtoActionResult UpdateComputer(EntityComputer computer)
        {
            computer.Name = computer.Name.ToUpper();
            var u = GetComputer(computer.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Computer Not Found", Id = 0};
            var validationResult = ValidateComputer(computer, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.ComputerRepository.Update(computer, computer.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = computer.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateComputer(EntityComputer computer, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

          

            return validationResult;
        }

        public List<EntityGroupMembership> GetAllGroupMemberships(int computerId)
        {
            return ectx.Uow.GroupMembershipRepository.Get(x => x.ComputerId == computerId);
        }

        public DtoProvisionHardware GetProvisionHardware(int computerId)
        {
            var dtoHardware = new DtoProvisionHardware();
            var bios = ectx.Uow.BiosInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var processor = ectx.Uow.ProcessorInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var system = ectx.Uow.ComputerSystemInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var nics = ectx.Uow.NicInventoryRepository.Get(x => x.ComputerId == computerId);

            if (bios == null)
                dtoHardware.SerialNumber = string.Empty;
            else
                dtoHardware.SerialNumber = bios.SerialNumber;

            if (processor == null)
                dtoHardware.Processor = string.Empty;
            else
                dtoHardware.Processor = processor.Name;

            if (system == null)
            {
                dtoHardware.Model = string.Empty;
                dtoHardware.Memory = 0;
            }
            else
            {
                dtoHardware.Model = system.Model;
                dtoHardware.Memory = system.Memory;
            }

            if(nics != null)
            {
                foreach(var nic in nics)
                {
                    dtoHardware.Macs.Add(nic.Mac);
                }
            }

            if (bios == null && processor == null && system == null)
                return null;

            return dtoHardware;


        }

        public DtoInventoryCollection GetSystemInfo(int computerId)
        {
            var systemInfo = new DtoInventoryCollection();
            systemInfo.Bios = ectx.Uow.BiosInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityBiosInventory();
            systemInfo.ComputerSystem = ectx.Uow.ComputerSystemInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityComputerSystemInventory();
            systemInfo.Gpu = ectx.Uow.ComputerGpuRepository.Get(x => x.ComputerId == computerId) ?? new List<EntityComputerGpuInventory>();
            systemInfo.Os = ectx.Uow.OsInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityOsInventory();
            systemInfo.Processor =
                ectx.Uow.ProcessorInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityProcessorInventory();
            systemInfo.HardDrives = ectx.Uow.HardDriveInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Printers = ectx.Uow.PrinterInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Nics = ectx.Uow.NicInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.AntiVirus = ectx.Uow.AntivirusRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Bitlocker = ectx.Uow.BitlockerRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Firewall = ectx.Uow.FirewallRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityFirewallInventory();
            systemInfo.LogicalVolume = ectx.Uow.LogicalVolumeRepository.Get(x => x.ComputerId == computerId);
            return systemInfo;   
        }

        public DtoActionResult AddComment(DtoComputerComment comment, int userId)
        {
            if (string.IsNullOrEmpty(comment.Comment))
            {
                return new DtoActionResult() { ErrorMessage = "Comments Cannot Be Empty" };
            }

            var user = userService.GetUser(userId);
            if (user == null)
                return new DtoActionResult() { ErrorMessage = "Could Not Determine Current User" };

            var entityComment = new EntityComment();
            entityComment.CommentText = comment.Comment;
            entityComment.CommentTime = DateTime.Now;
            entityComment.Username = user.Name;
            ectx.Uow.CommentRepository.Insert(entityComment);
            ectx.Uow.Save();

            var computerComment = new EntityComputerComment();
            computerComment.ComputerId = comment.ComputerId;
            computerComment.CommentId = entityComment.Id;
            ectx.Uow.ComputerCommentRepository.Insert(computerComment);
            ectx.Uow.Save();

            return new DtoActionResult() { Success = true, Id = computerComment.Id };
        }

        public List<DtoComputerComment> GetComments(int computerId)
        {
            var commentIds = ectx.Uow.ComputerCommentRepository.Get(x => x.ComputerId == computerId).Select(x => x.CommentId).ToList();
            if (commentIds.Count == 0) return new List<DtoComputerComment>();

            var list = new List<DtoComputerComment>();
            foreach (var commentId in commentIds)
            {
                var comment = ectx.Uow.CommentRepository.GetById(commentId);
                if (comment == null) continue;
                var computerComment = new DtoComputerComment();
                computerComment.Comment = comment.CommentText;
                computerComment.CommentTime = comment.CommentTime;
                computerComment.Username = comment.Username;
                list.Add(computerComment);
            }

            return list.OrderByDescending(x => x.CommentTime).ToList();
        }

        public List<EntityAttachment> GetAttachments(int computerId)
        {
            return ectx.Uow.AssetAttachmentRepository.GetComputerAttachments(computerId);
        }

        public List<DtoProcessWithTime> GetComputerProcessTimes(DateTime dateCutoff, int limit, int computerId)
        {
            return new ReportRepository().GetTopProcessTimesForComputer(dateCutoff, limit, computerId);
        }

        public List<DtoProcessWithCount> GetComputerProcessCounts(DateTime dateCutoff, int limit, int computerId)
        {
            return new ReportRepository().GetTopProcessCountsForComputer(dateCutoff, limit, computerId);
        }

        public List<DtoProcessWithUser> GetAllProcessForComputer(DateTime dateCutoff, int limit, int computerId)
        {
            return new ReportRepository().GetAllProcessForComputer(dateCutoff, limit, computerId);
        }

        public bool IsComputerActive(int computerId)
        {
            return ectx.Uow.ActiveImagingTaskRepository.Exists(a => a.ComputerId == computerId);
        }

        public EntityActiveImagingTask GetTaskForComputer(int computerId)
        {
            return ectx.Uow.ActiveImagingTaskRepository.GetFirstOrDefault(x => x.ComputerId == computerId);
        }

        public EntityActiveImagingTask GetTaskForComputerCheckin(int computerId)
        {
            return
                ectx.Uow.ActiveImagingTaskRepository.GetFirstOrDefault(
                    x =>
                        x.ComputerId == computerId &&
                        (x.Type == "upload" || x.Type == "deploy" ||
                         x.Type == "multicast"));
        }

        public string DeployImageViaWindows(int computerId, int userId)
        {
            ClearLastSocketResult(computerId);
            GetStatus(computerId);

            var winPeModule = GetEffectiveWinPeModule(computerId);
            if (winPeModule == null)
                return "This Computer Does Not Have A WinPE Module Assigned.";

            var counter = 0;
            while (counter < 10)
            {
                var socketResult = new UnitOfWork().ComputerRepository.GetById(computerId);
                if (socketResult == null)
                {
                    counter++;
                    continue;
                }
               
                if (!string.IsNullOrEmpty(socketResult.LastSocketResult))
                {
                    if (!socketResult.LastSocketResult.Equals("Connected"))
                        return "Could Not Connect To Computer.  Verify Toec Is Installed And Running On This Computer.";
                    else
                        break;
                }
                if (counter == 9)
                {
                    return "Could Not Connect To Computer.  Verify Toec Is Installed And Running On This Computer.";
                }
                System.Threading.Thread.Sleep(1000);
                counter++;
            }

            var startImagingTaskResult = new Toems_Service.Workflows.Unicast(computerId,"deploy",userId).Start();

            if (!startImagingTaskResult.Contains("Successfully"))
                return startImagingTaskResult;

            var computer = ectx.Uow.ComputerRepository.GetById(computerId);

            var moduleTypeMapping = new DtoGuidTypeMapping();

         
            
            moduleTypeMapping.moduleId = winPeModule.Id;
            moduleTypeMapping.moduleType = EnumModule.ModuleType.WinPE;

            var clientPolicy = new ClientPolicyJson().CreateInstantModule(moduleTypeMapping);

            var socket = ectx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ectx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ectx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "WinPE_Image";
                socketRequest.message = JsonConvert.SerializeObject(clientPolicy);
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return "Success";
        }

        public EntityComputer GetComputerFromClientIdentifier(string clientIdentifier)
        {
            if (string.IsNullOrEmpty(clientIdentifier))
                return null;
            EntityComputer result;
            clientIdentifier = clientIdentifier.ToUpper();
            //Don't know if uuid is raw or pretty.  Check for both
            var prettyIdentifier = "";
            try
            {
                var uuid = clientIdentifier.Substring(clientIdentifier.LastIndexOf('.') + 1);
                var clientIdFirst = clientIdentifier.Replace(uuid, string.Empty);
                var uuidGuid = new Guid(uuid);
                var uuidBytes = uuidGuid.ToByteArray();
                var strReverseUuid = "";
                foreach (var b in uuidBytes)
                {
                    strReverseUuid += b.ToString("X2");
                }
                var reverseUuid = new Guid(strReverseUuid);
                prettyIdentifier = (clientIdFirst + reverseUuid).ToUpper();
            }
            catch
            { //ignored
            }

            //check image only computers first
            result = ectx.Uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == clientIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly);
            if (result != null) return result;
            result = ectx.Uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == prettyIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly);
            if (result != null) return result;

            //Check provisiond computers next
            result = ectx.Uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == clientIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
            if (result != null) return result;
            result = ectx.Uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == prettyIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
            if (result != null) return result;

            //check in global list of id's for matching computer
            var computerClientIds = ectx.Uow.ClientImagingIdRepository.Get(x => x.ClientIdentifier == clientIdentifier);
            var matchingComps = new List<EntityComputer>();
            if(computerClientIds.Count > 0)
            {
                foreach(var clientId in computerClientIds)
                {
                    var computer = ectx.Uow.ComputerRepository.GetById(clientId.ComputerId);
                    if (computer.ProvisionStatus == EnumProvisionStatus.Status.Provisioned)
                        matchingComps.Add(computer);
                }
                //if no matches or more than 1 match, don't return anything, user will need to register an image only computer
                if (matchingComps.Count == 1)
                {
                    //since match was found update client identifier 
                    var mac = clientIdentifier.Split('.').First();
                    matchingComps.First().ImagingMac = mac;
                    matchingComps.First().ImagingClientId = clientIdentifier;
                    ectx.Uow.ComputerRepository.Update(matchingComps.First(), matchingComps.First().Id);
                    ectx.Uow.Save();
                    return matchingComps.First();
                    
                }
            }

            computerClientIds = ectx.Uow.ClientImagingIdRepository.Get(x => x.ClientIdentifier == prettyIdentifier);
            matchingComps.Clear();
            matchingComps = new List<EntityComputer>();
            if (computerClientIds.Count > 0)
            {
                foreach (var clientId in computerClientIds)
                {
                    var computer = ectx.Uow.ComputerRepository.GetById(clientId.ComputerId);
                    if (computer.ProvisionStatus == EnumProvisionStatus.Status.Provisioned)
                        matchingComps.Add(computer);
                }
                //if no matches or more than 1 match, don't return anything, user will need to register an image only computer
                if (matchingComps.Count == 1)
                {
                    //since match was found update client identifier 
                    var mac = clientIdentifier.Split('.').First();
                    matchingComps.First().ImagingMac = mac;
                    matchingComps.First().ImagingClientId = clientIdentifier;
                    ectx.Uow.ComputerRepository.Update(matchingComps.First(), matchingComps.First().Id);
                    ectx.Uow.Save();
                    return matchingComps.First();
                }
            }

            //no matches
            return null;

        }

        public List<EntityComputerLog> GetComputerLogs(int computerId)
        {
            return ectx.Uow.ComputerLogRepository.Get(x => x.ComputerId == computerId,
                q => q.OrderByDescending(x => x.LogTime));
        }
        
        //todo: remove
         //no longer used since Blazor UI
        public List<EntityComputer> Search(DtoSearchFilterCategories filter, int userId)
        {
            if(filter.Categories == null) filter.Categories = new List<string>();
            var list = ectx.Uow.ComputerRepository.SearchActiveComputers(filter,userId);
            
            if (!string.IsNullOrEmpty(filter.CategoryType) && filter.CategoryType != "Any Category")
            {
                var categoryFilterIds = filter.Categories
                    .Select(catName => ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName)))
                    .Where(category => category != null)
                    .Select(category => category.Id)
                    .ToList();

                var toRemove = new List<EntityComputer>();

                if (filter.CategoryType == "And Category")
                {
                    toRemove = list.Where(computer =>
                    {
                        var cCategories = GetComputerCategories(computer.Id);
                        if (cCategories == null) return true;
                        if (filter.Categories.Count == 0) return cCategories.Count > 0;
                        return categoryFilterIds.Any(id => !cCategories.Any(x => x.CategoryId == id));
                    }).ToList();
                }
                else if (filter.CategoryType == "Or Category")
                {
                    toRemove = list.Where(computer =>
                    {
                        var cCategories = GetComputerCategories(computer.Id);
                        if (cCategories == null) return true;
                        if (filter.Categories.Count == 0) return cCategories.Count > 0;
                        return !categoryFilterIds.Any(id => cCategories.Any(x => x.CategoryId == id));
                    }).ToList();
                }

                list.RemoveAll(x => toRemove.Contains(x));
            }
            
            foreach (var c in list)
            {
                var currentImage = GetEffectiveImage(c.Id);
                c.CurrentImage = currentImage?.Name;
            }


            var computerAcl = new ServiceUser().GetAllowedComputers(userId);
            return computerAcl.ComputerManagementEnforced
                ? list.Where(c => computerAcl.AllowedComputerIds.Contains(c.Id)).ToList()
                : list.ToList();

        }

        //todo: remove
        //no longer used since Blazor UI
        public List<EntityComputer> SearchImageOnlyComputers(DtoSearchFilterCategories filter, int userId)
        {
            var list = new List<EntityComputer>();
            if(filter.Categories == null) filter.Categories = new List<string>();
            var sortMode = userService.GetUserComputerSort(userId);

            if (sortMode == null)
                list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly).OrderByDescending(x => x.LastCheckinTime).ThenBy(x => x.Name).ToList();
            else if(sortMode.Equals("Last Checkin"))
                list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly).OrderByDescending(x => x.LastCheckinTime).ThenBy(x => x.Name).ToList();
            else if (sortMode.Equals("Name"))
                list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly).OrderBy(x => x.Name).ToList();
            else
                list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly).OrderByDescending(x => x.LastCheckinTime).ThenBy(x => x.Name).ToList();

            if (list.Count == 0) return list;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityComputer>();
            if (filter.CategoryType.Equals("Any Category") || filter.CategoryType.Equals(string.Empty) || filter.CategoryType == null)
            { 
                //do nothing
            }
            else if (filter.CategoryType.Equals("And Category"))
            {
                foreach (var computer in list)
                {
                    var cCategories = GetComputerCategories(computer.Id);
                    if (cCategories == null) continue;

                    if (filter.Categories.Count == 0)
                    {
                        if (cCategories.Count > 0)
                        {
                            toRemove.Add(computer);
                            continue;
                        }
                    }

                    foreach (var id in categoryFilterIds)
                    {
                        if (cCategories.Any(x => x.CategoryId == id)) continue;
                        toRemove.Add(computer);
                        break;
                    }
                }
            }
            else if (filter.CategoryType.Equals("Or Category"))
            {
                foreach (var computer in list)
                {
                    var cCategories = GetComputerCategories(computer.Id);
                    if (cCategories == null) continue;
                    if (filter.Categories.Count == 0)
                    {
                        if (cCategories.Count > 0)
                        {
                            toRemove.Add(computer);
                            continue;
                        }
                    }
                    var catFound = false;
                    foreach (var id in categoryFilterIds)
                    {
                        if (cCategories.Any(x => x.CategoryId == id))
                        {
                            catFound = true;
                            break;
                        }

                    }
                    if (!catFound)
                        toRemove.Add(computer);
                }
            }

            foreach (var p in toRemove)
            {
                list.Remove(p);
            }

            var computerAcl = userService.GetAllowedComputers(userId);
            if (!computerAcl.ComputerManagementEnforced)
                return list.Take(filter.Limit).ToList();
            else
            {
                var computers = new List<EntityComputer>();
                foreach (var c in list)
                {
                    if (computerAcl.AllowedComputerIds.Contains(c.Id))
                        computers.Add(c);
                }


                return computers.Take(filter.Limit).ToList();
            }

        }
        
        //todo: remove
          //no longer used since Blazor UI
        public List<EntityComputer> SearchAllComputers(DtoSearchFilterAllComputers filter, int userId)
        {
            var sortMode = userService.GetUserComputerSort(userId);
            var list = new List<EntityComputer>();
            if (filter.State.Equals("Enabled"))
                filter.State = "false";
            else if (filter.State.Equals("Disabled"))
                filter.State = "true";


            if (sortMode == null)
            {
                if (filter.State.Equals("Any State") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("Any State") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status).OrderBy(x => x.Name).ToList();

                else if (filter.State.Equals("true") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("false") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && !s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("true") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("false") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && !s.AdDisabled).OrderBy(x => x.Name).ToList();

            }
            else if (sortMode.Equals("Last Checkin"))
            {
                if (filter.State.Equals("Any State") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)).OrderByDescending(x => x.LastCheckinTime).ThenBy(x=> x.Name).ToList();
                else if (filter.State.Equals("Any State") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status).OrderByDescending(x => x.LastCheckinTime).ThenBy(x => x.Name).ToList();

                else if (filter.State.Equals("true") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && s.AdDisabled).OrderByDescending(x => x.LastCheckinTime).ThenBy(x => x.Name).ToList();
                else if (filter.State.Equals("false") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && !s.AdDisabled).OrderByDescending(x => x.LastCheckinTime).ThenBy(x => x.Name).ToList();
                else if (filter.State.Equals("true") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.AdDisabled).OrderByDescending(x => x.LastCheckinTime).ThenBy(x => x.Name).ToList();
                else if (filter.State.Equals("false") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && !s.AdDisabled).OrderByDescending(x => x.LastCheckinTime).ThenBy(x => x.Name).ToList();

            }
            else if (sortMode.Equals("Name"))
            {
                if (filter.State.Equals("Any State") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("Any State") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status).OrderBy(x => x.Name).ToList();

                else if (filter.State.Equals("true") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("false") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && !s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("true") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("false") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && !s.AdDisabled).OrderBy(x => x.Name).ToList();

            }
            else
            {
                if (filter.State.Equals("Any State") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("Any State") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status).OrderBy(x => x.Name).ToList();

                else if (filter.State.Equals("true") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("false") && !filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && !s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("true") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.AdDisabled).OrderBy(x => x.Name).ToList();
                else if (filter.State.Equals("false") && filter.Status.Equals("Any Status"))
                    list = ectx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.UUID.Contains(filter.SearchText) || s.ImagingClientId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && !s.AdDisabled).OrderBy(x => x.Name).ToList();

            }




            if (list.Count == 0) return list;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if(category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityComputer>();
            if (filter.CategoryType.Equals("Any Category"))
            {
                //ignore
            }
            else if (filter.CategoryType.Equals("And Category"))
            {
                foreach (var computer in list)
                {
                    var cCategories = GetComputerCategories(computer.Id);
                    if (cCategories == null) continue;

                    if (filter.Categories.Count == 0)
                    {
                        if (cCategories.Count > 0)
                        {
                            toRemove.Add(computer);
                            continue;
                        }
                    }

                    foreach (var id in categoryFilterIds)
                    {
                        if (cCategories.Any(x => x.CategoryId == id)) continue;
                        toRemove.Add(computer);
                        break;
                    }
                }
            }
            else if (filter.CategoryType.Equals("Or Category"))
            {
                foreach (var computer in list)
                {
                    var cCategories = GetComputerCategories(computer.Id);
                    if (cCategories == null) continue;
                    if (filter.Categories.Count == 0)
                    {
                        if (cCategories.Count > 0)
                        {
                            toRemove.Add(computer);
                            continue;
                        } 
                    }
                    var catFound = false;
                    foreach (var id in categoryFilterIds)
                    {
                        if (cCategories.Any(x => x.CategoryId == id))
                        {
                            catFound = true;
                            break;
                        }

                    }
                    if (!catFound)
                        toRemove.Add(computer);
                }
            }

            foreach (var p in toRemove)
            {
                list.Remove(p);
            }

            var computerAcl = userService.GetAllowedComputers(userId);
            if (!computerAcl.ComputerManagementEnforced)
                return list.Take(filter.Limit).ToList();
            else
            {
                var computers = new List<EntityComputer>();
                foreach (var c in list)
                {
                    if (computerAcl.AllowedComputerIds.Contains(c.Id))
                        computers.Add(c);
                }


                return computers.Take(filter.Limit).ToList();
            }
            

        }
    }
}