using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputer
    {
        private readonly UnitOfWork _uow;

        public ServiceComputer()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult RestoreComputer(int computerId)
        {
            var u = GetComputer(computerId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Computer Not Found", Id = 0 };
           u.ProvisionStatus = EnumProvisionStatus.Status.PreProvisioned;     
            u.ArchiveDateTime = null;
            u.Name = u.Name.Split('#').First();
            if(_uow.ComputerRepository.Exists(x => x.Name.Equals(u.Name)))
                return new DtoActionResult() { ErrorMessage = "Could Not Restore Computer.  A Computer With Name " + u.Name + " Already Exists"};
            _uow.ComputerRepository.Update(u, u.Id);
            _uow.Save();
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
            _uow.ComputerRepository.Update(u,u.Id);
            _uow.CertificateRepository.DeleteRange(x => x.Id == u.CertificateId);
            _uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == u.Id);
            u.CertificateId = -1;
            _uow.Save();
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
            _uow.ComputerRepository.Update(u, u.Id);
            _uow.CertificateRepository.DeleteRange(x => x.Id == u.CertificateId);
            u.CertificateId = -1;
            _uow.Save();
            return new DtoActionResult() { Id = u.Id, Success = true };
        }

        public DtoActionResult AddComputer(EntityComputer computer)
        {
            computer.Name = computer.Name.ToUpper();
            var validationResult = ValidateComputer(computer, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.ComputerRepository.Insert(computer);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = computer.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult DeleteComputer(int computerId)
        {
            var u = GetComputer(computerId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Computer Not Found", Id = 0};
            _uow.ComputerRepository.Delete(computerId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public List<EntityGroup> GetComputerGroups(int computerId)
        {
            return _uow.ComputerRepository.GetAllComputerGroups(computerId);
        }

        public List<EntityPolicy> GetComputerPolicies(int computerId)
        {
            return _uow.ComputerRepository.GetComputerPolicies(computerId);
        }

        public List<DtoComputerPolicyHistory> GetPolicyHistory(int computerId)
        {
            return _uow.ComputerRepository.GetPolicyHistory(computerId);
        }

        public List<DtoModule> GetComputerModules(int computerId)
        {
            return _uow.ComputerRepository.GetComputerModules(computerId);
        }


        public string GetEffectivePolicy(int computerId, EnumPolicy.Trigger trigger, string comServerUrl)
        {
            var computer = _uow.ComputerRepository.GetById(computerId);
            if (computer == null) return string.Empty;

            var policyRequest = new DtoPolicyRequest();
            policyRequest.Trigger = trigger;
            policyRequest.CurrentComServer = comServerUrl;
            policyRequest.ClientIdentity = new DtoClientIdentity();
            policyRequest.ClientIdentity.Guid = computer.Guid;
            policyRequest.ClientIdentity.Name = computer.Name;

            var policy = new Workflows.GetClientPolicies().Execute(policyRequest);
            return JsonConvert.SerializeObject(policy.Policies, Formatting.Indented);
        }

        public EntityComputer GetComputer(int computerId)
        {
            return _uow.ComputerRepository.GetById(computerId);
        }

        public List<EntityGroup> GetComputerAdGroups(int computerId)
        {
           return _uow.ComputerRepository.GetComputerAdGroups(computerId);
        }

        public EntityComputer GetByInstallationId(string installationid)
        {
            return _uow.ComputerRepository.GetFirstOrDefault(x => x.InstallationId == installationid);
        }

        public EntityComputer GetByName(string computerName)
        {
            return _uow.ComputerRepository.GetFirstOrDefault(x => x.Name == computerName && x.ProvisionStatus != EnumProvisionStatus.Status.Archived);
        }

        public EntityComputer GetByNameForReset(string computerName)
        {
            return _uow.ComputerRepository.GetFirstOrDefault(x => x.Name == computerName);
        }


        public EntityComputer GetByGuid(string computerGuid)
        {
            return _uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == computerGuid);
        }

        public List<EntityComputer> SearchAllComputers(DtoSearchFilterAllComputers filter)
        {
            var list = new List<EntityComputer>();
            if (filter.State.Equals("Enabled"))
                filter.State = "false";
            else if (filter.State.Equals("Disabled"))
                filter.State = "true";

            if(filter.State.Equals("Any State") && filter.Status.Equals("Any Status"))
                list = _uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
            else if(filter.State.Equals("Any State") && !filter.Status.Equals("Any Status"))
                list = _uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status).OrderBy(x => x.Name).ToList();
         
            else if (filter.State.Equals("true") && !filter.Status.Equals("Any Status"))
                list = _uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && s.AdDisabled).OrderBy(x => x.Name).ToList();
            else if (filter.State.Equals("false") && !filter.Status.Equals("Any Status"))
                list = _uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)) && s.ProvisionStatus.ToString() == filter.Status && !s.AdDisabled).OrderBy(x => x.Name).ToList();
            else if(filter.State.Equals("true") && filter.Status.Equals("Any Status"))
                list = _uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)) && s.AdDisabled).OrderBy(x => x.Name).ToList();
            else if (filter.State.Equals("false") && filter.Status.Equals("Any Status"))
                list = _uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)) && !s.AdDisabled).OrderBy(x => x.Name).ToList();

            if (list.Count == 0) return list;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = _uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if(category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityComputer>();
            if (filter.CategoryType.Equals("Any Category"))
                return list.Take(filter.Limit).ToList();
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

            return list.Take(filter.Limit).ToList();
        }

        public List<EntityComputerCategory> GetComputerCategories(int computerId)
        {
            return _uow.ComputerCategoryRepository.Get(x => x.ComputerId == computerId);
        }

        public List<EntityCustomComputerAttribute> GetCustomAttributes(int computerId)
        {
            return _uow.CustomComputerAttributeRepository.Get(x => x.ComputerId == computerId);
        }


        public List<EntityComputer> SearchComputers(DtoSearchFilterCategories filter)
        {
            var list = _uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText) || s.LastIp.Contains(filter.SearchText)) && s.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned && s.ProvisionStatus != EnumProvisionStatus.Status.Archived && s.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved).OrderByDescending(x => x.LastCheckinTime).ToList();
            if (list.Count == 0) return list;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = _uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityComputer>();
            if (filter.CategoryType.Equals("Any Category"))
                return list.Take(filter.Limit).ToList();
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

            return list.Take(filter.Limit).ToList();
        }

        public List<EntityComputer> GetArchived(DtoSearchFilterCategories filter)
        {
            return _uow.ComputerRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText) || s.InstallationId.Contains(filter.SearchText)) && s.ProvisionStatus == EnumProvisionStatus.Status.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityComputer> GetAllAdComputers()
        {
            return _uow.ComputerRepository.Get(s => s.IsAdSync);
        }

        public List<EntityComputer> SearchForGroup(DtoSearchFilter filter)
        {
            return _uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) && (s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned || s.ProvisionStatus == EnumProvisionStatus.Status.Provisioned || s.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly)).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityComputer> SearchPreProvision(DtoSearchFilter filter)
        {
            return _uow.ComputerRepository.Get(s => s.Name.Contains(filter.SearchText) && s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntitySoftwareInventory> GetComputerSoftware(int id, string searchString = "")
        {
            return _uow.ComputerRepository.GetComputerSoftware(id, searchString);
          
        }

        public List<EntityCertificateInventory> GetComputerCertificates(int id, string searchString = "")
        {
            return _uow.ComputerRepository.GetComputerCertificates(id, searchString);

        }

        public bool SendMessage(int id, DtoMessage message)
        {
            var computer = _uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = _uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Message";
                socketRequest.message = JsonConvert.SerializeObject(message);
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool ForceCheckin(int id)
        {
            var computer = _uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = _uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Force_Checkin";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        public bool CollectInventory(int id)
        {
            var computer = _uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var socket = _uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if(socket != null)
            {
                var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Collect_Inventory";
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey,socketRequest);
            }

            return true;
        }

        public string GetLoggedInUsers(int id)
        {
            var computer = _uow.ComputerRepository.GetById(id);
            if (computer == null) return string.Empty;
            if (computer.CertificateId == -1) return string.Empty;
            if (string.IsNullOrEmpty(computer.PushUrl)) return string.Empty;
            var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
            var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
            var result = new APICall().ClientApi.GetLoggedInUsers(computer.PushUrl, deviceCert);
            return result;
        }

        public bool GetStatus(int id)
        {
            var computer = _uow.ComputerRepository.GetById(id);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            if (string.IsNullOrEmpty(computer.PushUrl)) return false;
            var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
            var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
            return new APICall().ClientApi.GetStatus(computer.PushUrl, deviceCert);

        }

        public bool Reboot(int id)
        {
            return new Workflows.PowerManagement().RebootComputer(id);
        }

        public bool Shutdown(int id)
        {
            return new Workflows.PowerManagement().ShutdownComputer(id);
        }

        public bool Wakeup(int id)
        {
            new Workflows.PowerManagement().WakeupComputer(id);
            return true;
        }

        public List<DtoComputerUpdates> GetUpdates(int id, string searchString = "")
        {
            return _uow.ComputerRepository.GetWindowsUpdates(id, searchString);

        }

        public List<EntityUserLogin> GetUserLogins(int id, string searchString = "")
        {
            return _uow.UserLoginRepository.Get(x => x.ComputerId == id && x.UserName.Contains(searchString)).OrderByDescending(x => x.LoginDateTime).ToList();
        }

        public List<DtoCustomComputerInventory> GetCustomInventory(int id)
        {
            return _uow.ComputerRepository.GetCustomComputerInventory(id);
        }

        public string AllCount()
        {
            return _uow.ComputerRepository.Count();
        }

        public string TotalCount()
        {
            return _uow.ComputerRepository.Count(s => s.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned || s.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
        }

        public string TotalActiveCount()
        {
            return _uow.ComputerRepository.Count(s => s.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned && s.ProvisionStatus != EnumProvisionStatus.Status.Archived);
        }

        public string ArchivedCount()
        {
            return _uow.ComputerRepository.Count(s => s.ProvisionStatus == EnumProvisionStatus.Status.Archived);
        }

        public string TotalPreProvisionCount()
        {
            return _uow.ComputerRepository.Count(x => x.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned);
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
                _uow.ComputerRepository.Update(computer, computer.Id);
                _uow.Save();
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
            return _uow.GroupMembershipRepository.Get(x => x.ComputerId == computerId);
        }

        public DtoProvisionHardware GetProvisionHardware(int computerId)
        {
            var dtoHardware = new DtoProvisionHardware();
            var bios = _uow.BiosInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var processor = _uow.ProcessorInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var system = _uow.ComputerSystemInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            var nics = _uow.NicInventoryRepository.Get(x => x.ComputerId == computerId);

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
            systemInfo.Bios = _uow.BiosInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            systemInfo.ComputerSystem = _uow.ComputerSystemInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            systemInfo.Os = _uow.OsInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            systemInfo.Processor =
                _uow.ProcessorInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            systemInfo.HardDrives = _uow.HardDriveInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Printers = _uow.PrinterInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Nics = _uow.NicInventoryRepository.Get(x => x.ComputerId == computerId);
            systemInfo.AntiVirus = _uow.AntivirusRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Bitlocker = _uow.BitlockerRepository.Get(x => x.ComputerId == computerId);
            systemInfo.Firewall = _uow.FirewallRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            systemInfo.LogicalVolume = _uow.LogicalVolumeRepository.Get(x => x.ComputerId == computerId);
            return systemInfo;   
        }

        public DtoActionResult AddComment(DtoComputerComment comment, int userId)
        {
            if (string.IsNullOrEmpty(comment.Comment))
            {
                return new DtoActionResult() { ErrorMessage = "Comments Cannot Be Empty" };
            }

            var user = new ServiceUser().GetUser(userId);
            if (user == null)
                return new DtoActionResult() { ErrorMessage = "Could Not Determine Current User" };

            var entityComment = new EntityComment();
            entityComment.CommentText = comment.Comment;
            entityComment.CommentTime = DateTime.Now;
            entityComment.Username = user.Name;
            _uow.CommentRepository.Insert(entityComment);
            _uow.Save();

            var computerComment = new EntityComputerComment();
            computerComment.ComputerId = comment.ComputerId;
            computerComment.CommentId = entityComment.Id;
            _uow.ComputerCommentRepository.Insert(computerComment);
            _uow.Save();

            return new DtoActionResult() { Success = true, Id = computerComment.Id };
        }

        public List<DtoComputerComment> GetComments(int computerId)
        {
            var commentIds = _uow.ComputerCommentRepository.Get(x => x.ComputerId == computerId).Select(x => x.CommentId).ToList();
            if (commentIds.Count == 0) return new List<DtoComputerComment>();

            var list = new List<DtoComputerComment>();
            foreach (var commentId in commentIds)
            {
                var comment = _uow.CommentRepository.GetById(commentId);
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
            return _uow.AssetAttachmentRepository.GetComputerAttachments(computerId);
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
            return _uow.ActiveImagingTaskRepository.Exists(a => a.ComputerId == computerId);
        }

        public EntityActiveImagingTask GetTaskForComputer(int computerId)
        {
            return _uow.ActiveImagingTaskRepository.GetFirstOrDefault(x => x.ComputerId == computerId);
        }

        public EntityActiveImagingTask GetTaskForComputerCheckin(int computerId)
        {
            return
                _uow.ActiveImagingTaskRepository.GetFirstOrDefault(
                    x =>
                        x.ComputerId == computerId &&
                        (x.Type == "upload" || x.Type == "deploy" ||
                         x.Type == "multicast"));
        }

        public EntityComputer GetComputerFromClientIdentifier(string clientIdentifier)
        {
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
            result = _uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == clientIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly);
            if (result != null) return result;
            result = _uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == prettyIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.ImageOnly);
            if (result != null) return result;

            //Check provisiond computers next
            result = _uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == clientIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
            if (result != null) return result;
            result = _uow.ComputerRepository.GetFirstOrDefault(p => p.ImagingClientId == prettyIdentifier && p.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
            if (result != null) return result;

            //check in global list of id's for matching computer
            var computerClientIds = _uow.ClientImagingIdRepository.Get(x => x.ClientIdentifier == clientIdentifier);
            var matchingComps = new List<EntityComputer>();
            if(computerClientIds.Count > 0)
            {
                foreach(var clientId in computerClientIds)
                {
                    var computer = _uow.ComputerRepository.GetById(clientId.ComputerId);
                    if (computer.ProvisionStatus == EnumProvisionStatus.Status.Provisioned)
                        matchingComps.Add(computer);
                }
                //if no matches or more than 1 match, don't return anything, user will need to register an image only computer
                if (matchingComps.Count == 1)
                    return matchingComps.First();
            }

            computerClientIds = _uow.ClientImagingIdRepository.Get(x => x.ClientIdentifier == prettyIdentifier);
            matchingComps.Clear();
            matchingComps = new List<EntityComputer>();
            if (computerClientIds.Count > 0)
            {
                foreach (var clientId in computerClientIds)
                {
                    var computer = _uow.ComputerRepository.GetById(clientId.ComputerId);
                    if (computer.ProvisionStatus == EnumProvisionStatus.Status.Provisioned)
                        matchingComps.Add(computer);
                }
                //if no matches or more than 1 match, don't return anything, user will need to register an image only computer
                if (matchingComps.Count == 1)
                    return matchingComps.First();
            }

            //no matches
            return null;

        }
    }
}