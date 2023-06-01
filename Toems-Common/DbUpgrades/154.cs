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

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Default Image Replication', 'All');

ALTER TABLE `images` 
ADD COLUMN `replication_mode` TINYINT(4) NOT NULL DEFAULT 0 AFTER `image_type`;

CREATE TABLE `image_replication_servers` (
  `image_replication_server_id` INT NOT NULL AUTO_INCREMENT,
  `image_id` INT NOT NULL,
  `com_server_id` INT NOT NULL,
  PRIMARY KEY (`image_replication_server_id`));

ALTER TABLE `image_replication_servers` 
ADD INDEX `fk_irs_image_idx` (`image_id` ASC),
ADD INDEX `fk_irs_comserver_idx` (`com_server_id` ASC);
ALTER TABLE `image_replication_servers` 
ADD CONSTRAINT `fk_irs_image`
  FOREIGN KEY (`image_id`)
  REFERENCES `images` (`image_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_irs_comserver`
  FOREIGN KEY (`com_server_id`)
  REFERENCES `client_com_servers` (`client_com_server_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Image Replication Time', 'Scheduler');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Replication In Progress', 'False');

CREATE TABLE `default_image_replication_servers` (
  `default_image_replication_server_id` INT NOT NULL AUTO_INCREMENT,
  `com_server_id` INT NOT NULL,
  PRIMARY KEY (`default_image_replication_server_id`));

ALTER TABLE `default_image_replication_servers` 
ADD INDEX `dimrs_com_idx` (`com_server_id` ASC);
ALTER TABLE `default_image_replication_servers` 
ADD CONSTRAINT `dimrs_com`
  FOREIGN KEY (`com_server_id`)
  REFERENCES `client_com_servers` (`client_com_server_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Web UI Timeout', '60');

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Last Wie Guid', '');

CREATE TABLE `wie_builds` (
  `wie_build_id` INT NOT NULL AUTO_INCREMENT,
  `wie_guid` VARCHAR(45) NULL,
  `datetime_started` DATETIME NULL,
  `datetime_end` DATETIME NULL,
  `status` VARCHAR(45) NULL,
  `build_options` MEDIUMTEXT NULL,
  PRIMARY KEY (`wie_build_id`));

ALTER TABLE `wie_builds` 
ADD COLUMN `pid` VARCHAR(45) NULL AFTER `build_options`;

"


            ;
        }
    }
}
