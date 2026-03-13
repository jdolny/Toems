using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfileFileCopy(ServiceContext ctx)
    {
        public DtoActionResult AddImageProfileFileCopy(EntityImageProfileFileCopy imageProfileFileCopy)
        {
            ctx.Uow.ImageProfileFileCopyRepository.Insert(imageProfileFileCopy);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileFileCopy.Id;
            return actionResult;
        }
    }
}
