using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfileScript(ServiceContext ctx)
    {
        public DtoActionResult AddImageProfileScript(EntityImageProfileScript imageProfileScript)
        {
            ctx.Uow.ImageProfileScriptRepository.Insert(imageProfileScript);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileScript.Id;
            return actionResult;
        }
    }
}
