using System;
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

namespace Toems_ApplicationApi.Controllers
{
    public class ComServerClusterController : ApiController
    {
        private readonly ServiceComServerCluster _comServerClusterService;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ComServerClusterController()
        {
            _comServerClusterService = new ServiceComServerCluster();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var cluster = _comServerClusterService.GetCluster(id);
            var result = _comServerClusterService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "ComServerCluster";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = cluster.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(cluster);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Delete;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public EntityComServerCluster Get(int id)
        {
            var result = _comServerClusterService.GetCluster(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityComServerCluster> Get()
        {
            return _comServerClusterService.GetAll();
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityComServerClusterServer> GetClusterServers(int id)
        {
            return _comServerClusterService.GetClusterServers(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public IEnumerable<EntityComServerCluster> Search(DtoSearchFilter filter)
        {
            return _comServerClusterService.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _comServerClusterService.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntityComServerCluster cluster)
        {
            var result = _comServerClusterService.Add(cluster);
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "ComServerCluster";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = cluster.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(cluster);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Create;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntityComServerCluster cluster)
        {
            cluster.Id = id;
            var result = _comServerClusterService.Update(cluster);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "ComServerCluster";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = cluster.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(cluster);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Update;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }
    }
}