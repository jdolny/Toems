using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceAuditLog(ServiceContext ctx)
    {
        public void AddAuditLog(EntityAuditLog auditLog)
        {
            ctx.Uow.AuditLogRepository.Insert(auditLog);
            ctx.Uow.Save();
        }
    }
}