using Microsoft.AspNetCore.Mvc;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_UI.Services;

namespace Toems_UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComputerController(ServiceComputer computerService, CurrentUserService CurrentUser) : ControllerBase
    {
      [HttpPost]
      public async Task <IEnumerable<EntityComputer>> SearchComputers(DtoComputerFilter filter)
      {
          return null;
          // return await computerService.SearchComputers(filter,CurrentUser.UserIdInt);
      }
    }
}