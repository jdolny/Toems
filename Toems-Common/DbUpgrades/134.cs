using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _134 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.3.4.0', `database_version` = '1.3.4.0', `expected_toecapi_version` = '1.3.4.0', `latest_client_version` = '1.3.4.0' WHERE (`toems_version_id` = '1');

INSERT INTO `toemstest`.`admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES('Image Direct SMB', '0');"
            ;
        }
    }
}
