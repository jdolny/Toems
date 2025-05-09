using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CertificateInventoryAPI : BaseAPI<EntityCertificateInventory>
    {
        public CertificateInventoryAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }
    }
}