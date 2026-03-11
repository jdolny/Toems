using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfileSysprep(ServiceContext ctx)
    {
        public DtoActionResult AddImageProfileSysprep(EntityImageProfileSysprepTag imageProfileSysprep)
        {
            ctx.Uow.ImageProfileSysprepRepository.Insert(imageProfileSysprep);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileSysprep.Id;
            return actionResult;
        }
    }
}
