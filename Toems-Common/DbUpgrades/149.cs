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

CREATE TABLE `computer_gpu_inventory` (
  `computer_gpu_inventory_id` INT NOT NULL AUTO_INCREMENT,
  `computer_id` INT NOT NULL,
  `computer_gpu_name` VARCHAR(255) NULL DEFAULT NULL,
  `computer_gpu_ram` INT NULL DEFAULT 0,
  PRIMARY KEY (`computer_gpu_inventory_id`));

    ALTER TABLE `computer_gpu_inventory` 
ADD INDEX `CGPU_COMPUTER_FK_idx` (`computer_id` ASC);
ALTER TABLE `theopenem`.`computer_gpu_inventory` 
ADD CONSTRAINT `CGPU_COMPUTER_FK`
  FOREIGN KEY (`computer_id`)
  REFERENCES `theopenem`.`computers` (`computer_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

ALTER TABLE `groups` 
ADD COLUMN `is_security_group` TINYINT NULL DEFAULT NULL AFTER `proxy_bootloader`;

ALTER TABLE `groups` 
CHANGE COLUMN `is_ou` `is_ou` TINYINT(4) NULL DEFAULT 0 ,
CHANGE COLUMN `is_security_group` `is_security_group` TINYINT(4) NULL DEFAULT 0 ;

ALTER TABLE `groups` 
ADD COLUMN `is_hidden` TINYINT NULL DEFAULT 0 AFTER `is_security_group`;

ALTER TABLE `toems_users` 
ADD COLUMN `mfa_secret_encrypted` VARCHAR(255) NULL DEFAULT NULL AFTER `default_login_page`,

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Enable MFA', '0');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Force MFA', '0');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('MFA Display Name', '');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Force Imaging MFA', '0');

ALTER TABLE `toems_users` 
ADD COLUMN `enable_web_mfa` TINYINT NULL DEFAULT 0 AFTER `mfa_secret_encrypted`,
ADD COLUMN `enable_imaging_mfa` TINYINT NULL DEFAULT 0 AFTER `enable_web_mfa`;

ALTER TABLE `toems_users` 
ADD COLUMN `mfa_temp_secret_encrypted` VARCHAR(255) NULL DEFAULT NULL AFTER `enable_imaging_mfa`;

ALTER TABLE `active_imaging_tasks` 
ADD COLUMN `web_task_token` VARCHAR(100) NULL DEFAULT NULL AFTER `image_profile_id`;


"


            ;
        }
    }
}
