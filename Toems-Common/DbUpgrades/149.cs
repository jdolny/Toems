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

CREATE TABLE `toec_deploy_jobs` (
  `toec_deploy_job_id` INT NOT NULL AUTO_INCREMENT,
  `toec_deploy_job_domain` VARCHAR(45) NULL DEFAULT NULL,
  `toec_deploy_job_username` VARCHAR(45) NULL DEFAULT NULL,
  `toec_deploy_job_password_encrypted` VARCHAR(45) NULL DEFAULT NULL,
  `toec_deploy_target_list_id` INT NOT NULL,
  `toec_deploy_exclusion_list_id` INT NULL,
  `toec_deploy_job_type` TINYINT(4) NOT NULL DEFAULT 0,
  `toec_deploy_run_mode` TINYINT(4) NOT NULL DEFAULT 0,
  `toec_deploy_job_max_workers` TINYINT(4) NULL DEFAULT 1,
  `toec_deploy_job_is_enabled` TINYINT(4) NULL DEFAULT 0,
  PRIMARY KEY (`toec_deploy_job_id`));

CREATE TABLE `toec_deploy_target_lists` (
  `toec_deploy_target_list_id` INT NOT NULL AUTO_INCREMENT,
  `toec_deploy_target_list_name` VARCHAR(45) NOT NULL,
  `toec_deploy_target_list_type` TINYINT(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`toec_deploy_target_list_id`));

CREATE TABLE `toec_deploy_target_list_computers` (
  `toec_deploy_target_list_computer_id` INT NOT NULL AUTO_INCREMENT,
  `toec_deploy_target_list_computer_name` VARCHAR(45) NOT NULL,
  `toec_deploy_target_list_computer_status` TINYINT(4) NULL DEFAULT 0,
  `toec_deploy_target_list_id` INT NOT NULL DEFAULT -1,
  PRIMARY KEY (`toec_deploy_target_list_computer_id`));


CREATE TABLE `toec_deploy_target_list_ous` (
  `toec_deploy_target_list_ou_id` INT NOT NULL AUTO_INCREMENT,
  `toec_deploy_target_list_group_id` INT NOT NULL,
  `toec_deploy_target_list_id` INT NOT NULL,
  PRIMARY KEY (`toec_deploy_target_list_ou_id`));


ALTER TABLE `toec_deploy_target_list_computers` 
ADD INDEX `fk_target_list_idx` (`toec_deploy_target_list_id` ASC);
ALTER TABLE `toec_deploy_target_list_computers` 
ADD CONSTRAINT `fk_target_list`
  FOREIGN KEY (`toec_deploy_target_list_id`)
  REFERENCES `toec_deploy_target_lists` (`toec_deploy_target_list_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;


ALTER TABLE `toec_deploy_target_list_ous` 
ADD INDEX `fk_group_id_idx` (`toec_deploy_target_list_group_id` ASC),
ADD INDEX `fk_targetlist_id_idx` (`toec_deploy_target_list_id` ASC);
ALTER TABLE `toec_deploy_target_list_ous` 
ADD CONSTRAINT `fk_group_id`
  FOREIGN KEY (`toec_deploy_target_list_group_id`)
  REFERENCES `groups` (`group_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_targetlist_id`
  FOREIGN KEY (`toec_deploy_target_list_id`)
  REFERENCES `toec_deploy_target_lists` (`toec_deploy_target_list_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

ALTER TABLE `toec_deploy_jobs` 
ADD COLUMN `toec_deploy_job_name` VARCHAR(45) NOT NULL AFTER `toec_deploy_job_is_enabled`;

ALTER TABLE `toec_deploy_target_list_computers` 
ADD COLUMN `last_status_change_date` DATETIME NULL DEFAULT NULL AFTER `toec_deploy_target_list_id`;

CREATE TABLE `toec_deploy_threads` (
  `toec_deploy_thread_id` INT NOT NULL AUTO_INCREMENT,
  `toec_deploy_thread_job_id` INT NOT NULL,
  `toec_deploy_thread_task_id` INT NOT NULL,
  `toec_deploy_thread_datetime` DATETIME NULL,
  PRIMARY KEY (`toec_deploy_thread_id`));

ALTER TABLE `toec_deploy_threads` 
CHANGE COLUMN `toec_deploy_thread_task_id` `toec_deploy_thread_task_id` VARCHAR(45) NULL ;

ALTER TABLE `toec_deploy_target_list_computers` 
ADD COLUMN `last_update_details` TEXT NULL AFTER `last_status_change_date`;

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('Toec Remote Install Max Workers', '5');



"


            ;
        }
    }
}
