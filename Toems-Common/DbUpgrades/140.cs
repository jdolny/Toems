using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _140 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.4.0.0', `database_version` = '1.4.0.0', `expected_toecapi_version` = '1.4.0.0', `latest_client_version` = '1.4.0.0' WHERE (`toems_version_id` = '1');

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES('Image Direct SMB', '0');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Domain Join User', '');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Domain Join Password Encrypted', '');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Domain Join Name', '');

ALTER TABLE `policies` 
ADD COLUMN `join_domain` TINYINT(4) NULL DEFAULT 0 AFTER `policy_remote_access`,
ADD COLUMN `image_prep_cleanup` TINYINT(4) NULL DEFAULT 0 AFTER `join_domain`,
ADD COLUMN `domain_ou` VARCHAR(255) NULL AFTER `image_prep_cleanup`;


"
            ;
        }
    }
}
