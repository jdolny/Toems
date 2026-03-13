using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfileTemplate(ServiceContext ctx)
    {
        public EntityImageProfileTemplate GetTemplate(EnumProfileTemplate.TemplateType templateType)
        {
            return ctx.Uow.ImageProfileTemplateRepository.GetFirstOrDefault(x => x.TemplateType == templateType);
        }

        public DtoActionResult UpdateTemplate(EntityImageProfileTemplate template)
        {
            var actionResult = new DtoActionResult();
            var existingTemplate = GetTemplate(template.TemplateType);
            if (existingTemplate == null)
                return new DtoActionResult { ErrorMessage = "Template Not Found", Id = 0 };

            template.Id = existingTemplate.Id;
            ctx.Uow.ImageProfileTemplateRepository.Update(template, existingTemplate.Id);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = existingTemplate.Id;
            return actionResult;
        }
    }
}