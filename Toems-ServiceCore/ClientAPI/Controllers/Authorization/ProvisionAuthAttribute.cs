using Microsoft.AspNetCore.Mvc;

namespace Toems_ClientApi.Controllers.Authorization;

public class ProvisionAuthAttribute : TypeFilterAttribute
{
    public ProvisionAuthAttribute() : base(typeof(ProvisionAuthFilter))
    {
    }
}