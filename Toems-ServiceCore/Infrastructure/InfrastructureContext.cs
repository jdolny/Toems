using log4net;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure;

public class InfrastructureContext
{
    public ILog Log { get; }
    public IConfiguration Config { get; }
    public EncryptionServices Encryption { get; }
    public ServiceSetting Settings { get; }
    
    public ServiceAuditLog AuditLog { get; }
    
    public InfrastructureContext(IConfiguration config, ILog log, UnitOfWork uow, EncryptionServices encryption, ServiceSetting settings, ServiceAuditLog auditLog)   
    {
        AuditLog = auditLog;
        Config = config;
        Log = log;
        Encryption = encryption;
        Settings = settings;
    }
}