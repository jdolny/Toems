using Microsoft.AspNetCore.Mvc;

namespace Toems_ClientApi.Controllers.Authorization;

public class ClientImagingAuthAttribute : TypeFilterAttribute
{
    public ClientImagingAuthAttribute() : base(typeof(ClientImagingAuthFilter))
    {
    }
}