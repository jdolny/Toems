using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputerAttachment
    {
        private readonly UnitOfWork _uow;

        public ServiceComputerAttachment()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityComputerAttachment attachment)
        {
            var actionResult = new DtoActionResult();


            _uow.ComputerAttachmentRepository.Insert(attachment);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = attachment.Id;


            return actionResult;
        }






    }
}