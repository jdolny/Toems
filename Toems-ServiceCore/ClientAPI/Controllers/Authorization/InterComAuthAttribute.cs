using Microsoft.AspNetCore.Mvc;

namespace Toems_ClientApi.Controllers.Authorization;

public class InterComAuthAttribute : TypeFilterAttribute
{
    public InterComAuthAttribute() : base(typeof(InterComAuthFilter))
    {
    }
}