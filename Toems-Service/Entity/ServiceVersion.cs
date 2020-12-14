using System;
using Toems_Common;
using Toems_Common.DbUpgrades;
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

        public DtoVersion GetAllVersionInfo()
        {
            var versionDto = new DtoVersion();
            var dbversions = _uow.VersionRepository.GetById(1);
            versionDto.DatabaseVersion = dbversions.DatabaseVersion;
            var versionMapping = new VersionMapping().Get();
            versionDto.TargetDbVersion = versionMapping[SettingStrings.GlobalVersion];
            return versionDto;
        }
    }
}