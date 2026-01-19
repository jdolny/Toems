using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfileSysprep(EntityContext ectx)
    {
        public DtoActionResult AddImageProfileSysprep(EntityImageProfileSysprepTag imageProfileSysprep)
        {
            ectx.Uow.ImageProfileSysprepRepository.Insert(imageProfileSysprep);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = imageProfileSysprep.Id;
            return actionResult;
        }
    }
}
