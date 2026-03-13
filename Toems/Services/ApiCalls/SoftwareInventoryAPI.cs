using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class SoftwareInventoryAPI : BaseAPI<EntitySoftwareInventory>
    {
        public SoftwareInventoryAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }
    }
}