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

"


            ;
        }
    }
}
