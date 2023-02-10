﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Newtonsoft.Json;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ApplicationApi.Controllers
{
    public class ComputerController : ApiController
    {
        private readonly ServiceComputer _computerServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ComputerController()
        {
            _computerServices = new ServiceComputer();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerArchive)]
        [HttpGet]
        [ComputerAuth]
        public DtoActionResult Restore(int id)
        {
            var result = _computerServices.RestoreComputer(id);
            var computer = _computerServices.GetComputer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Restore;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerArchive)]
        [HttpGet]
        [ComputerAuth]
        public DtoActionResult Archive(int id)
        {
            var computer = _computerServices.GetComputer(id);
            var result = _computerServices.ArchiveComputer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Archive;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerUpdate)]
        [HttpGet]
        public DtoActionResult ClearImagingId(int id)
        {
            var result = _computerServices.ClearImagingClientId(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerDelete)]
        [ComputerAuth]
        public DtoActionResult Delete(int id)
        {
            var computer = _computerServices.GetComputer(id);
            var result = _computerServices.DeleteComputer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Delete;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [ComputerAuth]
        public EntityComputer Get(int id)
        {
            var result = _computerServices.GetComputer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityComputer> Get()
        {
            return _computerServices.SearchComputers(new DtoSearchFilterCategories(),_userId);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityGroup> GetComputerGroups(int id)
        {
            return _computerServices.GetComputerGroups(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<DtoGroupImage> GetComputerGroupsWithImage(int id)
        {
            return _computerServices.GetComputerGroupsWithImage(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityClientComServer> GetComputerEmServers(int id)
        {
            return _computerServices.GetEmServers(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityClientComServer> GetComputerTftpServers(int id)
        {
            return _computerServices.GetTftpServers(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityClientComServer> GetComputerImageServers(int id)
        {
            return _computerServices.GetImageServers(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public ImageProfileWithImage GetEffectiveImage(int id)
        {
            return _computerServices.GetEffectiveImage(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public EntityWinPeModule GetEffectiveWinPe(int id)
        {
            return _computerServices.GetEffectiveWinPeModule(id);
        }


        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<DtoComputerPolicyHistory> GetPolicyHistory(int id)
        {
            return _computerServices.GetPolicyHistory(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetEffectivePolicy(int id, EnumPolicy.Trigger trigger, string comServerUrl)
        {
            var result = _computerServices.GetEffectivePolicy(id, trigger, comServerUrl);
            return new DtoApiStringResponse() { Value = result };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityPolicy> GetComputerPolicies(int id)
        {
            return _computerServices.GetComputerPolicies(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<DtoModule> GetComputerModules(int id)
        {
            return _computerServices.GetComputerModules(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<DtoCustomComputerInventory> GetCustomInventory(int id)
        {
            return _computerServices.GetCustomInventory(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityCustomComputerAttribute> GetCustomAttributes(int id)
        {
            return _computerServices.GetCustomAttributes(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> Search(DtoSearchFilterCategories filter)
        {
            return _computerServices.SearchComputers(filter,_userId);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> SearchImageOnlyComputers(DtoSearchFilterCategories filter)
        {
            return _computerServices.SearchImageOnlyComputers(filter,_userId);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> SearchAllComputers(DtoSearchFilterAllComputers filter)
        {
            return _computerServices.SearchAllComputers(filter,_userId);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> GetArchived(DtoSearchFilterCategories filter)
        {
            return _computerServices.GetArchived(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> SearchPreProvision(DtoSearchFilter filter)
        {
            return _computerServices.SearchPreProvision(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> SearchForGroup(DtoSearchFilter filter)
        {
            return _computerServices.SearchForGroup(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        public DtoApiStringResponse ClearLastSocketResult(int id)
        {
            return new DtoApiStringResponse { Value = _computerServices.ClearLastSocketResult(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetLastSocketResult(int id)
        {
            return new DtoApiStringResponse { Value = _computerServices.LastSocketResult(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetActiveCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.TotalActiveCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetAllCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.AllCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.TotalCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetArchivedCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.ArchivedCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetImageOnlyCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.ImageOnlyCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetPreProvisionCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.TotalPreProvisionCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerUpdate)]
        public DtoActionResult Post(EntityComputer computer)
        {
            return _computerServices.AddComputer(computer);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerUpdate)]
        [ComputerAuth]
        public DtoActionResult Put(int id, EntityComputer computer)
        {
            computer.Id = id;
            var result = _computerServices.UpdateComputer(computer);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerSendMessage)]
        [HttpPost]
        [ComputerAuth]
        public DtoApiBoolResponse SendMessage(int id, DtoMessage message)
        {
            var response = new DtoApiBoolResponse() { Value = _computerServices.SendMessage(id, message) };
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(message);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Message;
                _auditLogService.AddAuditLog(auditLog);
            }
            return response;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerForceCheckin)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse ForceCheckin(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.ForceCheckin(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerForceCheckin)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse CollectInventory(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.CollectInventory(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerForceCheckin)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse GetUptime(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.GetSystemUptime(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse GetLoggedInUsers(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.GetLoggedInUsers(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse GetStatus(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.GetStatus(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse GetServiceLog(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.GetServiceLog(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerReboot)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse Reboot(int id)
        {
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Reboot;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _computerServices.Reboot(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.AllowRemoteControl)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse StartRemoteControl(int id)
        {
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.RemoteControl;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _computerServices.StartRemoteControl(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerShutdown)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse Shutdown(int id)
        {
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Shutdown;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _computerServices.Shutdown(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerWakeup)]
        [HttpGet]
        [ComputerAuth]
        public DtoApiBoolResponse Wakeup(int id)
        {
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Wakeup;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _computerServices.Wakeup(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        [ComputerAuth]
        public DtoInventoryCollection GetSystemInfo(int id)
        {
            return _computerServices.GetSystemInfo(id);
        }


        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        [ComputerAuth]
        public IEnumerable<EntitySoftwareInventory> GetSoftware(int id, string searchstring = "")
        {
            return string.IsNullOrEmpty(searchstring)
                ? _computerServices.GetComputerSoftware(id)
                : _computerServices.GetComputerSoftware(id, searchstring);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        [ComputerAuth]
        public IEnumerable<EntityCertificateInventory> GetCertificates(int id, string searchstring = "")
        {
            return string.IsNullOrEmpty(searchstring)
                ? _computerServices.GetComputerCertificates(id)
                : _computerServices.GetComputerCertificates(id, searchstring);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        [ComputerAuth]
        public IEnumerable<DtoComputerUpdates> GetUpdates(int id, string searchstring = "")
        {
            return string.IsNullOrEmpty(searchstring)
                ? _computerServices.GetUpdates(id)
                : _computerServices.GetUpdates(id, searchstring);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        [ComputerAuth]
        public IEnumerable<EntityUserLogin> GetUserLogins(int id, string searchstring = "")
        {
            return string.IsNullOrEmpty(searchstring)
                ? _computerServices.GetUserLogins(id)
                : _computerServices.GetUserLogins(id, searchstring);
        }

        [CustomAuth(Permission = AuthorizationStrings.CommentAdd)]
        [HttpPost]

        public DtoActionResult AddComment(DtoComputerComment comment)
        {
            return _computerServices.AddComment(comment, _userId);
        }

        [CustomAuth(Permission = AuthorizationStrings.CommentRead)]
        [ComputerAuth]
        public List<DtoComputerComment> GetComments(int id)
        {
            return _computerServices.GetComments(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.AttachmentRead)]
        [ComputerAuth]
        public IEnumerable<EntityAttachment> GetAttachments(int id)
        {
            return _computerServices.GetAttachments(id);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithTime> ComputerProcessTimes(DateTime dateCutoff, int limit, int computerId)
        {
            return _computerServices.GetComputerProcessTimes(dateCutoff, limit, computerId);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithCount> ComputerProcessCounts(DateTime dateCutoff, int limit, int computerId)
        {
            return _computerServices.GetComputerProcessCounts(dateCutoff, limit, computerId);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithUser> ComputerProcess(DateTime dateCutoff, int limit, int computerId)
        {
            return _computerServices.GetAllProcessForComputer(dateCutoff, limit, computerId);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ImageDeployTask)]
        [ComputerAuth]
        public DtoApiStringResponse StartDeploy(int id)
        {
            return new DtoApiStringResponse
            {
                Value = new Unicast(id, "deploy", _userId).Start()
            };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ImageDeployTask)]
        [ComputerAuth]

        public DtoApiStringResponse StartDeployWinPe(int id)
        {
            return new DtoApiStringResponse
            {
                Value = _computerServices.DeployImageViaWindows(id,_userId)
            };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ImageUploadTask)]
        [ComputerAuth]
        public DtoApiStringResponse StartUpload(int id)
        {
            return new DtoApiStringResponse
            {
                Value = new Unicast(id, "upload", _userId).Start()
            };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [ComputerAuth]
        public IEnumerable<EntityComputerLog> GetComputerImagingLogs(int id)
        {
            return _computerServices.GetComputerLogs(id);
        }


        [CustomAuth(Permission = AuthorizationStrings.ComputerUpdate)]
        [HttpGet]
        public DtoApiBoolResponse RunModule(int computerId, string moduleGuid)
        {
            return new DtoApiBoolResponse
            {
                Value = _computerServices.RunModule(computerId, moduleGuid)
            };
        }
    }
}