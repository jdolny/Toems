using System;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceVersion
    {
        private readonly UnitOfWork _uow;

        public ServiceVersion()
        {
            _uow = new UnitOfWork();
        }

        public bool FirstRunCompleted()
        {
            return Convert.ToBoolean(_uow.VersionRepository.GetById(1).FirstRunCompleted);
        }

        public EntityVersion GetVersions()
        {
            return _uow.VersionRepository.GetById(1);
        }

        public EntityVersion Get(int versionId)
        {
            return _uow.VersionRepository.GetById(versionId);
        }

        public DtoActionResult Update(EntityVersion version)
        {
            _uow.VersionRepository.Update(version, version.Id);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = version.Id;
            return actionResult;
        }
    }
}