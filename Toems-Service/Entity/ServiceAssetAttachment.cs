using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAssetAttachment
    {
        private readonly UnitOfWork _uow;

        public ServiceAssetAttachment()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityAssetAttachment attachment)
        {
            var actionResult = new DtoActionResult();


            _uow.AssetAttachmentRepository.Insert(attachment);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = attachment.Id;


            return actionResult;
        }






    }
}