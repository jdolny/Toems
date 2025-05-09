using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class WolRelayAPI : BaseAPI<EntityWolRelay>
    {
        public WolRelayAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }
    }
}