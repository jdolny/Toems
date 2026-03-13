using Toems_Common;
using Toems_Common.DbUpgrades;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceVersion(ServiceContext ctx)
    {
        public bool FirstRunCompleted()
        {
            return Convert.ToBoolean(ctx.Uow.VersionRepository.GetById(1).FirstRunCompleted);
        }

        public EntityVersion GetVersions()
        {
            return ctx.Uow.VersionRepository.GetById(1);
        }

        public EntityVersion Get(int versionId)
        {
            return ctx.Uow.VersionRepository.GetById(versionId);
        }

        public DtoActionResult Update(EntityVersion version)
        {
            ctx.Uow.VersionRepository.Update(version, version.Id);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = version.Id;
            return actionResult;
        }

        public DtoVersion GetAllVersionInfo()
        {
            var versionDto = new DtoVersion();
            var dbversions = ctx.Uow.VersionRepository.GetById(1);
            versionDto.DatabaseVersion = dbversions.DatabaseVersion;
            var versionMapping = new VersionMapping().Get();
            versionDto.TargetDbVersion = versionMapping[SettingStrings.GlobalVersion];
            return versionDto;
        }
    }
}