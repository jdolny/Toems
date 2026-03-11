using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputer(ServiceContext ctx)
    {
        public List<EntityComputer> GetComputers()
        {
            return ctx.Uow.ComputerRepository.Get();
        }
        
        public List<EntityComputer> SearchComputers(DtoComputerFilter filter, int userId)
        {
            if(filter.Categories == null) filter.Categories = new List<string>();
            var categoryFilterIds = filter.Categories
                   .Select(catName => ctx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName)))
                   .Where(category => category != null)
                   .Select(category => category.Id)
                   .ToList();

            var list = ctx.Uow.ComputerRepository.SearchAllComputers(filter,userId,categoryFilterIds);
            
            
            
            foreach (var c in list)
            {
                var currentImage = GetEffectiveImage(c.Id);
                c.CurrentImage = currentImage?.Name;
            }


            var computerAcl = ctx.User.GetAllowedComputers(userId);
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
            if(ctx.Uow.ComputerRepository.Exists(x => x.Name.Equals(u.Name)))
                return new DtoActionResult() { ErrorMessage = "Could Not Restore Computer.  A Computer With Name " + u.Name + " Already Exists"};
            ctx.Uow.ComputerRepository.Update(u, u.Id);
            ctx.Uow.Save();
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
            ctx.Uow.ComputerRepository.Update(u,u.Id);
            ctx.Uow.CertificateRepository.DeleteRange(x => x.Id == u.CertificateId);
            ctx.Uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == u.Id);
            ctx.Uow.NicInventoryRepository.DeleteRange(x => x.ComputerId == u.Id);
            u.CertificateId = -1;
            ctx.Uow.Save();
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
            ctx.Uow.ComputerRepository.Update(u, u.Id);
            ctx.Uow.CertificateRepository.DeleteRange(x => x.Id == u.CertificateId);
            u.CertificateId = -1;
            ctx.Uow.Save();
            return new DtoActionResult() { Id = u.Id, Success = true };
        }

        public DtoActionResult AddComputer(EntityComputer computer)
        {
            computer.Name = computer.Name.ToUpper();
            var validationResult = ValidateComputer(computer, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ctx.Uow.ComputerRepository.Insert(computer);
                ctx.Uow.Save();
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
            ctx.Uow.ComputerRepository.Update(u, u.Id);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public DtoActionResult DeleteComputer(int computerId)
        {
            var u = GetComputer(computerId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Computer Not Found", Id = 0};
            ctx.Uow.ComputerRepository.Delete(computerId);
            ctx.Uow.ComputerLogRepository.DeleteRange(x => x.ComputerId == computerId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public List<EntityGroup> GetComputerGroups(int computerId)
        {
            return ctx.Uow.ComputerRepository.GetAllComputerGroups(computerId);
        }

        public List<DtoGroupImage> GetComputerGroupsWithImage(int computerId)
        {
            return ctx.Uow.ComputerRepository.GetAllComputerGroupsWithImage(computerId);
        }

        public List<EntityPolicy> GetComputerPolicies(int computerId)
        {
            return ctx.Uow.ComputerRepository.GetComputerPolicies(computerId);
        }

        public List<DtoComputerPolicyHistory> GetPolicyHistory(int computerId)
        {
            return ctx.Uow.ComputerRepository.GetPolicyHistory(computerId);
        }

        public List<DtoModule> GetComputerModules(int computerId)
        {
            return ctx.Uow.ComputerRepository.GetComputerModules(computerId);
        }

        public List<EntityWingetModule> GetComputerWingetUpgrades(string clientGuid)
        {
            var client = ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == clientGuid);
            return ctx.Uow.ComputerRepository.GetComputerWingetUpdateModules(client.Id);
        }

        public EntityWinPeModule GetEffectiveWinPeModule(int computerId)
        {
            var computer = GetComputer(computerId);
            var winPeModule = ctx.WinPeModule.GetModule(computer.WinPeModuleId);
            if (winPeModule != null) return winPeModule;

            //check for an image profile via group since computer doesn't have image directly assigned
            var computerGroups = ctx.Uow.ComputerRepository.GetAllComputerGroups(computerId).OrderBy(x => x.ImagingPriority).ThenBy(x => x.Name).ToList();

            if (computerGroups.Count == 0)
            {
                return null;
            }
            else
            {
                foreach (var group in computerGroups)
                {
                    winPeModule = ctx.WinPeModule.GetModule(group.WinPeModuleId);
                    if (winPeModule != null) return winPeModule;
                }

                //no images assigned to any groups
                return null;
            }

        }

        public ImageProfileWithImage GetEffectiveImage(int computerId)
        {
            var computer = GetComputer(computerId);
            var imageProfile = ctx.ImageProfile.ReadProfile(computer.ImageProfileId);
            if (imageProfile != null) return imageProfile;
            
            var computerGroups = ctx.Uow.ComputerRepository.GetAllComputerGroups(computerId).OrderBy(x => x.ImagingPriority).ThenBy(x => x.Name).ToList();

            if (computerGroups.Count == 0)
            {
                return null;
            }
            else
            {
                foreach (var group in computerGroups)
                {
                    imageProfile = ctx.ImageProfile.ReadProfile(group.ImageProfileId);
                    if (imageProfile != null) return imageProfile;
                }

                //no images assigned to any groups
                return null;  
            }
        }

        public string GetEffectivePolicy(int computerId, EnumPolicy.Trigger trigger, string comServerUrl)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(computerId);
            if (computer == null) return string.Empty;

            var policyRequest = new DtoPolicyRequest();
            policyRequest.Trigger = trigger;
            policyRequest.CurrentComServer = comServerUrl;
            policyRequest.ClientIdentity = new DtoClientIdentity();
            policyRequest.ClientIdentity.Guid = computer.Guid;
            policyRequest.ClientIdentity.Name = computer.Name;

            var policy = ctx.GetClientPolicies.Execute(policyRequest,computerId);
            return JsonConvert.SerializeObject(policy.Policies, Formatting.Indented);
        }

        public EntityComputer GetComputer(int computerId)
        {
            return ctx.Uow.ComputerRepository.GetById(computerId);
        }

        public List<EntityGroup> GetComputerAdGroups(int computerId)
        {
           return ctx.Uow.ComputerRepository.GetComputerAdGroups(computerId);
        }

        public List<EntityGroup> GetComputerAdSecurityGroups(int computerId)
        {
            return ctx.Uow.ComputerRepository.GetComputerAdSecurityGroups(computerId);
        }

        public List<EntityClientComServer> GetEmServers(int computerId)
        {
            var list = new List<EntityClientComServer>();
            var computer = GetComputer(computerId);
            var result = new GetCompEmServers().Run(computer.Guid);
            foreach(var r in result)
            {
                list.Add(ctx.Uow.ClientComServerRepository.GetById(r.ComServerId));
            }
            return list;
        }

        public List<EntityClientComServer> GetTftpServers(int computerId)
        {
            return ctx.GetCompTftpServers.Run(computerId);
        }

        public List<EntityClientComServer> GetImageServers(int computerId)
        {
            return ctx.GetCompImagingServers.Run(computerId,true);
        }

        public EntityComputer GetByInstallationId(string installationid)
        {
            return ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.InstallationId == installationid);
        }

        public EntityComputer GetByName(string computerName)
        {
            return ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Name == computerName && x.ProvisionStatus != EnumProvisionStatus.Status.Archived);
        }

        public EntityComputer GetByNameForReset(string computerName)
        {
            return ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Name == computerName);
        }


        public EntityComputer GetByGuid(string computerGuid)
        {
            return ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == computerGuid);
        }

      

        public List<EntityComputerCategory> GetComputerCategories(int computerId)
        {
            return ctx.Uow.ComputerCategoryRepository.Get(x => x.ComputerId == computerId);
        }

        public List<EntityCustomComputerAttribute> GetCustomAttributes(int computerId)
        {
            return ctx.Uow.CustomComputerAttributeRepository.Get(x => x.ComputerId == computerId);
        }


       

        public List<EntityComputer> GetArchived(DtoSearchFilterCategories filter)
        {
            return ctx.Uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)) && s.ProvisionStatus == EnumProvisionStatus.Status.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityComputer> GetAllAdComputers()
        {
            return ctx.Uow.ComputerRepository.Get(s => s.IsAdSync);
        }

        public List<EntityComputer> SearchForGroup(DtoSearchFilter filter)
        {
            return ctx.Uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) && (s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned || s.ProvisionStatus == EnumProvisionStatus.Status.Provisioned || s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly)).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityComputer> SearchPreProvision(DtoSearchFilter filter)
        {
            return ctx.Uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) && s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntitySoftwareInventory> GetComputerSoftware(int id, string searchString = "")
        {
            return ctx.Uow.ComputerRepository.GetComputerSoftware(id, searchString);
          
        }

        public List<EntityCertificateInventory> GetComputerCertificates(int id, string searchString = "")
        {
            return ctx.Uow.ComputerRepository.GetComputerCertificates(id, searchString);

        }

        public DtoActionResult UpdateSocketResult(string result, string clientIdentity)
        {
            var client = ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == clientIdentity);
            if (client == null) return new DtoActionResult() { ErrorMessage = "Client Not Found", Success = false };
            client.LastSocketResult = result;
            ctx.Uow.ComputerRepository.Update(client, client.Id);
            ctx.Uow.Save();
            return new DtoActionResult() { Success = true, Id = client.Id };
        }

        public bool SendMessage(int id, DtoMessage message)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
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
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Start_Remote_Control";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool GetSystemUptime(int id)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "System_Uptime";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool RunModule(int computerId, string moduleGuid)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(computerId);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;

            var module = ctx.Module.GetModuleIdFromGuid(moduleGuid);
            if (module == null) return false;
            var clientPolicy = ctx.ClientPolicyJson.CreateInstantModule(module);

            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
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
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Force_Checkin";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool CollectInventory(int id)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if(socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Collect_Inventory";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey,socketRequest);
            }

            return true;
        }

        public bool GetLoggedInUsers(int id)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Current_Users";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool GetStatus(int id)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Get_Status";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;

        }

        public bool GetServiceLog(int id)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
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
            return ctx.PowerManagement.RebootComputer(id);
        }

        public bool Shutdown(int id)
        {
            return ctx.PowerManagement.ShutdownComputer(id);
        }

        public bool Wakeup(int id)
        {
            ctx.PowerManagement.WakeupComputer(id);
            return true;
        }

        public List<DtoComputerUpdates> GetUpdates(int id, string searchString = "")
        {
            return ctx.Uow.ComputerRepository.GetWindowsUpdates(id, searchString);

        }

        public List<EntityUserLogin> GetUserLogins(int id, string searchString = "")
        {
            return ctx.Uow.UserLoginRepository.Get(x => x.ComputerId == id && x.UserName.Contains(searchString)).OrderByDescending(x => x.LoginDateTime).ToList();
        }

        public List<DtoCustomComputerInventory> GetCustomInventory(int id)
        {
            return ctx.Uow.ComputerRepository.GetCustomComputerInventory(id);
        }

        public string AllCount()
        {
            return ctx.Uow.ComputerRepository.Count();
        }

        public string TotalCount()
        {
            return ctx.Uow.ComputerRepository.Count(s => s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned || s.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
        }

        public string TotalActiveCount()
        {
            return ctx.Uow.ComputerRepository.Count(s => s.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned && s.ProvisionStatus != EnumProvisionStatus.Status.Archived && s.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved && s.ProvisionStatus != EnumProvisionStatus.Status.ImageOnly);
        }

        public string ClearLastSocketResult(int id)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return string.Empty;
            computer.LastSocketResult = string.Empty;
            ctx.Uow.ComputerRepository.Update(computer, computer.Id);
            ctx.Uow.Save();
            return computer.LastSocketResult;
        }

        public string LastSocketResult(int id)
        {
            var computer = ctx.Uow.ComputerRepository.GetById(id);
            if (computer == null) return string.Empty;
            return computer.LastSocketResult;
        }

        public string ArchivedCount()
        {
            return ctx.Uow.ComputerRepository.Count(s => s.ProvisionStatus == EnumProvisionStatus.Status.Archived);
        }

        public string ImageOnlyCount()
        {
            return ctx.Uow.ComputerRepository.Count(s => s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly);
        }

        public string TotalPreProvisionCount()
        {
            return ctx.Uow.ComputerRepository.Count(x => x.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned);
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
                ctx.Uow.ComputerRepository.Update(computer, computer.Id);
                ctx.Uow.Save();
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
            return ctx.Uow.GroupMembershipRepository.Get(x => x.ComputerId == computerId);
        }

        public DtoProvisionHardware GetProvisionHardware(int computerId)
        {
            var dtoHardware = new DtoProvisionHardware();
            var bios = ctx.Uow.BiosInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var processor = ctx.Uow.ProcessorInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var system = ctx.Uow.ComputerSystemInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var nics = ctx.Uow.NicInventoryRepository.Get(x => x.ComputerId == computerId);

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
            systemInfo.Bios = ctx.Uow.BiosInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityBiosInventory();
            systemInfo.ComputerSystem = ctx.Uow.ComputerSystemInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityComputerSystemInventory();
            systemInfo.Gpu = ctx.Uow.ComputerGpuRepository.Get(x => x.ComputerId == computerId) ?? new List<EntityComputerGpuInventory>();
            systemInfo.Os = ctx.Uow.OsInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityOsInventory();
            systemInfo.Processor =
                ctx.Uow.ProcessorInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityProcessorInventory();
            systemInfo.HardDrives = ctx.Uow.HardDriveInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Printers = ctx.Uow.PrinterInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Nics = ctx.Uow.NicInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.AntiVirus = ctx.Uow.AntivirusRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Bitlocker = ctx.Uow.BitlockerRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Firewall = ctx.Uow.FirewallRepository.Get(x => x.ComputerId == computerId).FirstOrDefault() ?? new EntityFirewallInventory();
            systemInfo.LogicalVolume = ctx.Uow.LogicalVolumeRepository.Get(x => x.ComputerId == computerId);
            return systemInfo;   
        }

        public DtoActionResult AddComment(DtoComputerComment comment, int userId)
        {
            if (string.IsNullOrEmpty(comment.Comment))
            {
                return new DtoActionResult() { ErrorMessage = "Comments Cannot Be Empty" };
            }

            var user = ctx.User.GetUser(userId);
            if (user == null)
                return new DtoActionResult() { ErrorMessage = "Could Not Determine Current User" };

            var entityComment = new EntityComment();
            entityComment.CommentText = comment.Comment;
            entityComment.CommentTime = DateTime.Now;
            entityComment.Username = user.Name;
            ctx.Uow.CommentRepository.Insert(entityComment);
            ctx.Uow.Save();

            var computerComment = new EntityComputerComment();
            computerComment.ComputerId = comment.ComputerId;
            computerComment.CommentId = entityComment.Id;
            ctx.Uow.ComputerCommentRepository.Insert(computerComment);
            ctx.Uow.Save();

            return new DtoActionResult() { Success = true, Id = computerComment.Id };
        }

        public List<DtoComputerComment> GetComments(int computerId)
        {
            var commentIds = ctx.Uow.ComputerCommentRepository.Get(x => x.ComputerId == computerId).Select(x => x.CommentId).ToList();
            if (commentIds.Count == 0) return new List<DtoComputerComment>();

            var list = new List<DtoComputerComment>();
            foreach (var commentId in commentIds)
            {
                var comment = ctx.Uow.CommentRepository.GetById(commentId);
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
            return ctx.Uow.AssetAttachmentRepository.GetComputerAttachments(computerId);
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
            return ctx.Uow.ActiveImagingTaskRepository.Exists(a => a.ComputerId == computerId);
        }

        public EntityActiveImagingTask GetTaskForComputer(int computerId)
        {
            return ctx.Uow.ActiveImagingTaskRepository.GetFirstOrDefault(x => x.ComputerId == computerId);
        }

        public EntityActiveImagingTask GetTaskForComputerCheckin(int computerId)
        {
            return
                ctx.Uow.ActiveImagingTaskRepository.GetFirstOrDefault(
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
            
            ctx.Unicast.InitSingle(computerId,"deploy",userId);
            var startImagingTaskResult = ctx.Unicast.Start();


            if (!startImagingTaskResult.Contains("Successfully"))
                return startImagingTaskResult;

            var computer = ctx.Uow.ComputerRepository.GetById(computerId);

            var moduleTypeMapping = new DtoGuidTypeMapping();

         
            
            moduleTypeMapping.moduleId = winPeModule.Id;
            moduleTypeMapping.moduleType = EnumModule.ModuleType.WinPE;

            var clientPolicy = ctx.ClientPolicyJson.CreateInstantModule(moduleTypeMapping);

            var socket = ctx.Uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
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
            result = ctx.Uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == clientIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly);
            if (result != null) return result;
            result = ctx.Uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == prettyIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly);
            if (result != null) return result;

            //Check provisiond computers next
            result = ctx.Uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == clientIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
            if (result != null) return result;
            result = ctx.Uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == prettyIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
            if (result != null) return result;

            //check in global list of id's for matching computer
            var computerClientIds = ctx.Uow.ClientImagingIdRepository.Get(x => x.ClientIdentifier == clientIdentifier);
            var matchingComps = new List<EntityComputer>();
            if(computerClientIds.Count > 0)
            {
                foreach(var clientId in computerClientIds)
                {
                    var computer = ctx.Uow.ComputerRepository.GetById(clientId.ComputerId);
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
                    ctx.Uow.ComputerRepository.Update(matchingComps.First(), matchingComps.First().Id);
                    ctx.Uow.Save();
                    return matchingComps.First();
                    
                }
            }

            computerClientIds = ctx.Uow.ClientImagingIdRepository.Get(x => x.ClientIdentifier == prettyIdentifier);
            matchingComps.Clear();
            matchingComps = new List<EntityComputer>();
            if (computerClientIds.Count > 0)
            {
                foreach (var clientId in computerClientIds)
                {
                    var computer = ctx.Uow.ComputerRepository.GetById(clientId.ComputerId);
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
                    ctx.Uow.ComputerRepository.Update(matchingComps.First(), matchingComps.First().Id);
                    ctx.Uow.Save();
                    return matchingComps.First();
                }
            }

            //no matches
            return null;

        }

        public List<EntityComputerLog> GetComputerLogs(int computerId)
        {
            return ctx.Uow.ComputerLogRepository.Get(x => x.ComputerId == computerId,
                q => q.OrderByDescending(x => x.LogTime));
        }
        
       

       
    }
}