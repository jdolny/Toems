using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _156 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.5.6.0', `database_version` = '1.5.6.0', `expected_toecapi_version` = '1.5.6.0', `latest_client_version` = '1.5.4.0' WHERE (`toems_version_id` = '1');

CREATE TABLE `winget_manifest_downloads` (
  `winget_manifest_download_id` INT NOT NULL AUTO_INCREMENT,
  `url` VARCHAR(255) NOT NULL,
  `progress` VARCHAR(45) NULL,
  `status` TINYINT NULL DEFAULT 0,
  `error_message` TEXT NULL,
  `date_downloaded_local` DATETIME NULL,
  PRIMARY KEY (`winget_manifest_download_id`));

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Winget Manifest Cron', '0 5 * * *');

CREATE TABLE `winget_version_manifests` (
  `winget_version_manifest_id` INT NOT NULL AUTO_INCREMENT,
  `package_identifier` VARCHAR(128) NULL,
  `package_version` VARCHAR(128) NULL,
  `default_locale` VARCHAR(20) NULL,
  PRIMARY KEY (`winget_version_manifest_id`));


CREATE TABLE `winget_locale_manifests` (
  `winget_locale_manifest_id` INT NOT NULL AUTO_INCREMENT,
  `package_identifier` VARCHAR(128) NULL,
  `package_version` VARCHAR(128) NULL,
  `package_locale` VARCHAR(20) NULL,
  `publisher` VARCHAR(256) NULL,
  `publisher_url` VARCHAR(2048) NULL,
  `package_name` VARCHAR(256) NULL,
  `package_url` VARCHAR(2048) NULL,
  `license` VARCHAR(512) NULL,
  `short_description` VARCHAR(256) NULL,
  `manifest_type` VARCHAR(45) NULL,
  `tags` VARCHAR(700) NULL,
  `moniker` VARCHAR(40) NULL,
  PRIMARY KEY (`winget_locale_manifest_id`));

CREATE TABLE `winget_installer_manifests` (
  `winget_installer_manifest_id` INT NOT NULL AUTO_INCREMENT,
  `package_identifier` VARCHAR(128) NULL,
  `package_version` VARCHAR(128) NULL,
  `scope` VARCHAR(45) NULL,
  PRIMARY KEY (`winget_installer_manifest_id`));

CREATE TABLE `winget_modules` (
  `winget_module_id` INT NOT NULL AUTO_INCREMENT,
  `winget_module_guid` VARCHAR(45) NOT NULL,
  `winget_module_name` VARCHAR(45) NOT NULL,
  `winget_module_description` TEXT NULL DEFAULT NULL,
  `winget_module_arguments` VARCHAR(255) NULL,
  `winget_module_override` VARCHAR(255) NULL,
  `winget_package_id` VARCHAR(128) NULL,
  `winget_package_version` VARCHAR(128) NULL,
  `winget_install_type` TINYINT NULL,
  `winget_keep_updated` TINYINT NULL,
  `is_archived` TINYINT NULL DEFAULT 0,
  `datetime_archived_local` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`winget_module_id`));

ALTER TABLE `winget_modules` 
ADD INDEX `FK_WGM_GUID_idx` (`winget_module_guid` ASC);
ALTER TABLE `winget_modules` 
ADD CONSTRAINT `FK_WGM_GUID`
  FOREIGN KEY (`winget_module_guid`)
  REFERENCES `modules` (`module_guid`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

ALTER TABLE `winget_modules` 
ADD COLUMN `winget_module_timeout` TINYINT NULL AFTER `datetime_archived_local`,
ADD COLUMN `redirect_stdout` TINYINT NULL AFTER `winget_module_timeout`,
ADD COLUMN `redirect_stderror` TINYINT NULL AFTER `redirect_stdout`,
ADD COLUMN `impersonation_id` INT NULL DEFAULT -1 AFTER `redirect_stderror`;

ALTER TABLE `winget_locale_manifests` 
ADD COLUMN `major` INT NULL DEFAULT 0 AFTER `moniker`,
ADD COLUMN `minor` INT NULL DEFAULT 0 AFTER `major`,
ADD COLUMN `build` INT NULL DEFAULT 0 AFTER `minor`,
ADD COLUMN `revision` INT NULL DEFAULT 0 AFTER `build`;

ALTER TABLE `winget_modules` 
ADD COLUMN `winget_install_latest` TINYINT NULL DEFAULT 0 AFTER `impersonation_id`;

ALTER TABLE `policies` 
ADD COLUMN `is_winget_update` TINYINT NULL DEFAULT 0 AFTER `domain_ou`,
ADD COLUMN `winget_use_max_connections` TINYINT NULL DEFAULT 0 AFTER `is_winget_update`;

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Winget Package Source', 'https://github.com/microsoft/winget-pkgs/archive/refs/heads/master.zip');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('LIE Sleep Time', '5');

"
            ;
        }
    }
}
