using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceAuditLog(EntityContext ectx)
    {
        public void AddAuditLog(EntityAuditLog auditLog)
        {
            ectx.Uow.AuditLogRepository.Insert(auditLog);
            ectx.Uow.Save();
        }
    }
}