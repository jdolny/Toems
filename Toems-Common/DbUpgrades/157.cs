using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _157 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.5.7.0', `database_version` = '1.5.7.0', `expected_toecapi_version` = '1.5.7.0', `latest_client_version` = '1.5.6.0' WHERE (`toems_version_id` = '1');

ALTER TABLE `computers` 
ADD COLUMN `computer_description` TEXT NULL DEFAULT NULL AFTER `winpe_module_id`;

CREATE TABLE `toems_users_options` (
  `toems_users_options_id` INT NOT NULL AUTO_INCREMENT,
  `toems_user_id` INT NOT NULL,
  `description_enabled` TINYINT NULL DEFAULT 0,
  `description_order` TINYINT NULL DEFAULT 0,
  `last_checkin_enabled` TINYINT NULL DEFAULT 1,
  `last_checkin_order` TINYINT NULL DEFAULT 1,
  `last_ip_enabled` TINYINT NULL DEFAULT 1,
  `last_ip_order` TINYINT NULL DEFAULT 2,
  `client_version_enabled` TINYINT NULL DEFAULT 1,
  `client_version_order` TINYINT NULL DEFAULT 3,
  `last_user_enabled` TINYINT NULL DEFAULT 1,
  `last_user_order` TINYINT NULL DEFAULT 4,
  `provision_date_enabled` TINYINT NULL DEFAULT 1,
  `provision_date_order` TINYINT NULL DEFAULT 5,
  `status_enabled` TINYINT NULL DEFAULT 1,
  `status_order` TINYINT NULL DEFAULT 6,
  `current_image_enabled` TINYINT NULL DEFAULT 0,
  `current_image_order` TINYINT NULL DEFAULT 0,
  `manufacturer_enabled` TINYINT NULL DEFAULT 0,
  `manufacturer_order` TINYINT NULL DEFAULT 0,
  `model_enabled` TINYINT NULL DEFAULT 0,
  `model_order` TINYINT NULL DEFAULT 0,
  `os_name_enabled` TINYINT NULL DEFAULT 0,
  `os_name_order` TINYINT NULL DEFAULT 0,
  `os_version_enabled` TINYINT NULL DEFAULT 0,
  `os_version_order` TINYINT NULL DEFAULT 0,
  `os_build_enabled` TINYINT NULL DEFAULT 0,
  `os_build_order` TINYINT NULL DEFAULT 0,
  `domain_enabled` TINYINT NULL DEFAULT 0,
  `domain_order` TINYINT NULL DEFAULT 0,
  `force_checkin_enabled` TINYINT NULL DEFAULT 0,
  `force_checkin_order` TINYINT NULL DEFAULT 0,
  `collect_inventory_enabled` TINYINT NULL DEFAULT 0,
  `collect_inventory_order` TINYINT NULL DEFAULT 0,
  `remote_control_enabled` TINYINT NULL DEFAULT 0,
  `remote_control_order` TINYINT NULL DEFAULT 0,
  `service_log_enabled` TINYINT NULL DEFAULT 0,
  `service_log_order` TINYINT NULL DEFAULT 0,
  PRIMARY KEY (`toems_users_options_id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `browser_download_tokens` (
  `browser_download_token_id` INT NOT NULL AUTO_INCREMENT,
  `token` VARCHAR(45) NOT NULL,
  `expires_at_utc` DATETIME NOT NULL,
  PRIMARY KEY (`browser_download_token_id`));

ALTER TABLE `browser_download_tokens` 
ADD COLUMN `user_id` INT NOT NULL AFTER `expires_at_utc`;

"
            ;
        }
    }
}
