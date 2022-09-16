using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _149 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.4.9.0', `database_version` = '1.4.9.0', `expected_toecapi_version` = '1.4.9.0', `latest_client_version` = '1.4.9.0' WHERE (`toems_version_id` = '1');

ALTER TABLE `computer_system_inventory` 
ADD COLUMN `gpu` VARCHAR(255) NULL DEFAULT NULL AFTER `memory`;


"
            ;
        }
    }
}
