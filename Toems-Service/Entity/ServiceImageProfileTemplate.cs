using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceImageProfileTemplate
    {
        private readonly UnitOfWork _uow;

        public ServiceImageProfileTemplate()
        {
            _uow = new UnitOfWork();
        }

        public EntityImageProfileTemplate GetTemplate(EnumProfileTemplate.TemplateType templateType)
        {
            return _uow.ImageProfileTemplateRepository.GetFirstOrDefault(x => x.TemplateType == templateType);
        }

        public DtoActionResult UpdateTemplate(EntityImageProfileTemplate template)
        {
            var actionResult = new DtoActionResult();
            var existingTemplate = GetTemplate(template.TemplateType);
            if (existingTemplate == null)
                return new DtoActionResult { ErrorMessage = "Template Not Found", Id = 0 };

            template.Id = existingTemplate.Id;
            _uow.ImageProfileTemplateRepository.Update(template, existingTemplate.Id);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = existingTemplate.Id;
            return actionResult;
        }
    }
}