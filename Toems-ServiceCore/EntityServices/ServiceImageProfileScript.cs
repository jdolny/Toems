using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfileScript(EntityContext ectx)
    {
        public DtoActionResult AddImageProfileScript(EntityImageProfileScript imageProfileScript)
        {
            ectx.Uow.ImageProfileScriptRepository.Insert(imageProfileScript);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileScript.Id;
            return actionResult;
        }
    }
}
