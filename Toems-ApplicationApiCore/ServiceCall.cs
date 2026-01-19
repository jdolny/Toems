

using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ApplicationApiCore;

public class ServiceCall
{
    public AuthenticationServices AuthenticationServices => new();
    public ServiceSetting SettingService => new();
    public ServiceUser UserService => new();
}