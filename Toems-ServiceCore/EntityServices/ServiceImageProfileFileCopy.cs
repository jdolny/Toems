using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfileFileCopy(EntityContext ectx)
    {
        public DtoActionResult AddImageProfileFileCopy(EntityImageProfileFileCopy imageProfileFileCopy)
        {
            ectx.Uow.ImageProfileFileCopyRepository.Insert(imageProfileFileCopy);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileFileCopy.Id;
            return actionResult;
        }
    }
}
