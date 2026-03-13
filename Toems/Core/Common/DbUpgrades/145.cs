using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _145 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.4.5.0', `database_version` = '1.4.5.0', `expected_toecapi_version` = '1.4.5.0', `latest_client_version` = '1.4.4.0' WHERE (`toems_version_id` = '1');

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES('Default Computer View', 'Active');

"
            ;
        }
    }
}
