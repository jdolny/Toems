using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common;
using Toems_Common.DbUpgrades;
using Toems_Common.Dto;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class DbUpdater
    {
        private readonly ServiceRawSql _rawSqlServices;
        private readonly ILog log = LogManager.GetLogger(typeof(DbUpdater));

        public DbUpdater()
        {
            _rawSqlServices = new ServiceRawSql();
        }

        public DtoActionResult Update()
        {
            var versionService = new ServiceVersion();
            var versions = versionService.GetAllVersionInfo();
            if (versions.DatabaseVersion == versions.TargetDbVersion)
                return new DtoActionResult { Success = true };

            var result = new DtoActionResult();

            var updatesToRun = new List<int>();
            var currentDbVersion = versionService.Get(1).DatabaseVersion;
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
