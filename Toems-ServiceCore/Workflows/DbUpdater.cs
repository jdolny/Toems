using log4net;
using Toems_Common;
using Toems_Common.DbUpgrades;
using Toems_Common.Dto;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class DbUpdater(ServiceContext ctx)
    {
        private readonly ServiceRawSql _rawSqlServices = new();
        private readonly ILog log = LogManager.GetLogger(typeof(DbUpdater));
        
        public DtoActionResult Update()
        {
            var versions = ctx.Version.GetAllVersionInfo();
            if (versions.DatabaseVersion == versions.TargetDbVersion)
                return new DtoActionResult { Success = true };

            var result = new DtoActionResult();

            var updatesToRun = new List<int>();
            var currentDbVersion = ctx.Version.Get(1).DatabaseVersion;
            var currentAppVersion = SettingStrings.GlobalVersion;
            var versionMapping = new VersionMapping().Get();
            var targetDbVersion = versionMapping[currentAppVersion];

            var trimmedCurrent = currentDbVersion.Remove(currentDbVersion.Length - 1, 1);
            var currentInt = Convert.ToInt32(trimmedCurrent.Replace(".", ""));

            var trimmedTarget = targetDbVersion.Remove(targetDbVersion.Length - 1, 1);
            var targetInt = Convert.ToInt32(trimmedTarget.Replace(".", ""));

            if (targetDbVersion != currentDbVersion)
            {
                foreach (var v in versionMapping)
                {
                    var trimmedValue = v.Value.Remove(v.Value.Length - 1, 1);
                    var valueInt = Convert.ToInt32(trimmedValue.Replace(".", ""));

                    if (valueInt > currentInt && valueInt <= targetInt)
                    {
                        updatesToRun.Add(valueInt);
                    }
                }

            }

            var ordered = updatesToRun.OrderBy(x => x).Distinct().ToList();

            foreach (var version in ordered)
            {
                var type = Type.GetType("Toems_Common.DbUpgrades._" + version + ", Toems-Common");

                try
                {
                    var instance = Activator.CreateInstance(type) as IDbScript;
                    _rawSqlServices.ExecuteQuery(instance.Get());
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    log.Error("Could Not Update Database To Version " + version);
                    log.Error(ex.Message);
                    result.Success = false;
                    result.ErrorMessage = "Could Not Update Database To Version " + version + "<br>" + ex.Message;
                    return result;
                }
            }
            return result;
        }
    }
}
