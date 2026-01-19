using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfileTemplate(EntityContext ectx)
    {
        public EntityImageProfileTemplate GetTemplate(EnumProfileTemplate.TemplateType templateType)
        {
            return ectx.Uow.ImageProfileTemplateRepository.GetFirstOrDefault(x => x.TemplateType == templateType);
        }

        public DtoActionResult UpdateTemplate(EntityImageProfileTemplate template)
        {
            var actionResult = new DtoActionResult();
            var existingTemplate = GetTemplate(template.TemplateType);
            if (existingTemplate == null)
                return new DtoActionResult { ErrorMessage = "Template Not Found", Id = 0 };

            template.Id = existingTemplate.Id;
            ectx.Uow.ImageProfileTemplateRepository.Update(template, existingTemplate.Id);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = existingTemplate.Id;
            return actionResult;
        }
    }
}