using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _148 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.4.8.0', `database_version` = '1.4.8.0', `expected_toecapi_version` = '1.4.8.0', `latest_client_version` = '1.4.4.0' WHERE (`toems_version_id` = '1');

ALTER TABLE `toems_users` 
ADD COLUMN `default_computer_view` VARCHAR(45) NULL DEFAULT 'Default' AFTER `imaging_token`,
ADD COLUMN `computer_sort_mode` VARCHAR(45) NULL DEFAULT 'Default' AFTER `default_computer_view`,
ADD COLUMN `default_login_page` VARCHAR(45) NULL DEFAULT 'Default' AFTER `computer_sort_mode`;

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Computer Sort Mode', 'Last Checkin');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Default Login Page', 'Dashboard');

ALTER TABLE `software_inventory` 
ADD COLUMN `uninstall_string` TEXT NULL AFTER `revision`;

ALTER TABLE `user_logins` 
ADD COLUMN `client_login_id` INT(11) NULL AFTER `logout_date_time_utc`;

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Ldap Sync OU', '');

ALTER TABLE `groups` 
CHANGE COLUMN `group_name` `group_name` VARCHAR(255) NOT NULL ;

"
            ;
        }
    }
}
