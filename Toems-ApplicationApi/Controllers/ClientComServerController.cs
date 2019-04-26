using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public class ClientComServerController : ApiController
    {
        private readonly ServiceClientComServer _clientComService;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ClientComServerController()
        {
            _clientComService = new ServiceClientComServer();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
          
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var comServer = _clientComService.GetServer(id);
            var result = _clientComService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "ClientComServer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = comServer.DisplayName;
                auditLog.ObjectJson = JsonConvert.SerializeObject(comServer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Delete;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public EntityClientComServer Get(int id)
        {
            var result = _clientComService.GetServer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [Authorize]
        public IEnumerable<EntityClientComServer> Get()
        {
            return _clientComService.GetAll();
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public IEnumerable<EntityClientComServer> Search(DtoSearchFilter filter)
        {
            return _clientComService.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _clientComService.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntityClientComServer server)
        {
            var result = _clientComService.Add(server);
            if (result == null) return new DtoActionResult() { ErrorMessage = "Result Was Null" };
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "ClientComServer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = server.DisplayName;
                auditLog.ObjectJson = JsonConvert.SerializeObject(server);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Create;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntityClientComServer account)
        {
            account.Id = id;
            var result = _clientComService.Update(account);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "ClientComServer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = account.DisplayName;
                auditLog.ObjectJson = JsonConvert.SerializeObject(account);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Update;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public HttpResponseMessage GenerateCert(int id)
        {
            var cert = _clientComService.GenerateComCert(id);
            var dataStream = new MemoryStream(cert);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(dataStream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "Certificate.pfx";
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = dataStream.Length;
            return result;
        }
    }
}