using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerAttachment(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityComputerAttachment attachment)
        {
            var actionResult = new DtoActionResult();


            ctx.Uow.ComputerAttachmentRepository.Insert(attachment);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = attachment.Id;


            return actionResult;
        }






    }
}