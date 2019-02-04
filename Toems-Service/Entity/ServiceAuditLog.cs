using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAuditLog
    {
        private readonly UnitOfWork _uow;

        public ServiceAuditLog()
        {
            _uow = new UnitOfWork();
        }

        public void AddAuditLog(EntityAuditLog auditLog)
        {
            _uow.AuditLogRepository.Insert(auditLog);
            _uow.Save();
        }
    }
}