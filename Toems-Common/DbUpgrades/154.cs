using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _154 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.5.4.0', `database_version` = '1.5.4.0', `expected_toecapi_version` = '1.5.4.0', `latest_client_version` = '1.5.0.0' WHERE (`toems_version_id` = '1');

ALTER TABLE `image_profiles` 
ADD COLUMN `set_bootmgr_first` TINYINT(4) NOT NULL DEFAULT 1 AFTER `skip_bitlocker_check`;

ALTER TABLE `filecopy_modules` 
ADD COLUMN `is_driver` TINYINT(4) NOT NULL DEFAULT 0 AFTER `filecopy_overwrite`;


"
            ;
        }
    }
}
