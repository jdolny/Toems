using Microsoft.AspNetCore.Mvc;

namespace Toems_ClientApi.Controllers.Authorization;

public class CertificateAuthAttribute() : TypeFilterAttribute(typeof(CertificateAuthFilter));