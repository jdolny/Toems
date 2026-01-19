using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerAttachment(EntityContext ectx)
    {
        public DtoActionResult Add(EntityComputerAttachment attachment)
        {
            var actionResult = new DtoActionResult();


            ectx.Uow.ComputerAttachmentRepository.Insert(attachment);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = attachment.Id;


            return actionResult;
        }






    }
}