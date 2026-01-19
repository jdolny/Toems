using log4net;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure;

public class EntityContext
{
    public ILog Log { get; }
    public UnitOfWork Uow { get; }
    public IConfiguration Config { get; }
    public EncryptionServices Encryption { get; }
    public ServiceSetting Settings { get; }
    
    public EntityContext(IConfiguration config, ILog log, UnitOfWork uow, EncryptionServices encryption, ServiceSetting settings)   
    {
        Config = config;
        Log = log;
        Uow = uow;
        Encryption = encryption;
        Settings = settings;
    }
}