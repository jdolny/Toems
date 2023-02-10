using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _153 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.5.3.0', `database_version` = '1.5.3.0', `expected_toecapi_version` = '1.5.3.0', `latest_client_version` = '1.5.0.0' WHERE (`toems_version_id` = '1');


CREATE TABLE `toems_usergroup_memberships` (
  `usergroup_membership_id` INT NOT NULL AUTO_INCREMENT,
  `user_group_id` INT NOT NULL,
  `user_id` INT NOT NULL,
  PRIMARY KEY (`usergroup_membership_id`))
DEFAULT CHARACTER SET = utf8;

ALTER TABLE `toems_usergroup_memberships` 
ADD INDEX `userfk_idx` (`user_id` ASC),
ADD INDEX `usergroupfk_idx` (`user_group_id` ASC);
ALTER TABLE `toems_usergroup_memberships` 
ADD CONSTRAINT `userfk`
  FOREIGN KEY (`user_id`)
  REFERENCES `toems_users` (`toems_user_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION,
ADD CONSTRAINT `usergroupfk`
  FOREIGN KEY (`user_group_id`)
  REFERENCES `toems_user_groups` (`toems_user_group_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

ALTER TABLE `toems_user_groups` 
ADD COLUMN `enable_image_acls` TINYINT NULL DEFAULT 0 AFTER `toems_user_group_ldapname`,
ADD COLUMN `enable_computergroup_acls` TINYINT NULL DEFAULT 0 AFTER `enable_image_acls`;

CREATE TABLE `toems_user_groups_image_acls` (
  `toems_user_groups_image_acls_id` INT NOT NULL AUTO_INCREMENT,
  `user_group_id` INT NOT NULL,
  `image_id` INT NOT NULL,
  PRIMARY KEY (`toems_user_groups_image_acls_id`))
DEFAULT CHARACTER SET = utf8;

ALTER TABLE `toems_user_groups_image_acls` 
ADD INDEX `FK_UG_IACLS_idx` (`image_id` ASC),
ADD INDEX `FK_UG_CGACLS_idx` (`user_group_id` ASC);
ALTER TABLE `theopenem`.`toems_user_groups_image_acls` 
ADD CONSTRAINT `FK_UG_IACLS`
  FOREIGN KEY (`image_id`)
  REFERENCES `theopenem`.`images` (`image_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION,
ADD CONSTRAINT `FK_UG_uGACLS`
  FOREIGN KEY (`user_group_id`)
  REFERENCES `theopenem`.`toems_user_groups` (`toems_user_group_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

CREATE TABLE `toems_user_groups_computer_acls` (
  `toems_user_groups_computer_acls_id` INT NOT NULL AUTO_INCREMENT,
  `user_group_id` INT NOT NULL,
  `group_id` INT NOT NULL,
  PRIMARY KEY (`toems_user_groups_computer_acls_id`))
DEFAULT CHARACTER SET = utf8;

ALTER TABLE `toems_user_groups_computer_acls` 
ADD INDEX `FK_UG_CGACLS_idx` (`group_id` ASC),
ADD INDEX `FK_UG1_UGACLS_idx` (`user_group_id` ASC);
ALTER TABLE `theopenem`.`toems_user_groups_computer_acls` 
ADD CONSTRAINT `FK_UG_CGACLS`
  FOREIGN KEY (`group_id`)
  REFERENCES `theopenem`.`groups` (`group_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION,
ADD CONSTRAINT `FK_UG1_UGACLS`
  FOREIGN KEY (`user_group_id`)
  REFERENCES `theopenem`.`toems_user_groups` (`toems_user_group_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

CREATE TABLE `toems_users_images` (
  `toems_users_image_id` INT NOT NULL AUTO_INCREMENT,
  `user_id` INT NOT NULL,
  `image_id` INT NOT NULL,
  PRIMARY KEY (`toems_users_image_id`))
DEFAULT CHARACTER SET = utf8;

ALTER TABLE `toems_users_images` 
ADD INDEX `fk_tui_user_idx` (`user_id` ASC),
ADD INDEX `fk_tui_image_idx` (`image_id` ASC);
ALTER TABLE `toems_users_images` 
ADD CONSTRAINT `fk_tui_user`
  FOREIGN KEY (`user_id`)
  REFERENCES `toems_users` (`toems_user_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_tui_image`
  FOREIGN KEY (`image_id`)
  REFERENCES `images` (`image_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;

CREATE TABLE `toems_users_groups` (
  `toems_users_group_id` INT NOT NULL AUTO_INCREMENT,
  `group_id` INT NOT NULL,
  `user_id` INT NOT NULL,
  PRIMARY KEY (`toems_users_group_id`))
DEFAULT CHARACTER SET = utf8;

ALTER TABLE `toems_users_groups` 
ADD INDEX `fk_tug_group_idx` (`group_id` ASC),
ADD INDEX `fk_tug_user_idx` (`user_id` ASC);
ALTER TABLE `toems_users_groups` 
ADD CONSTRAINT `fk_tug_group`
  FOREIGN KEY (`group_id`)
  REFERENCES `groups` (`group_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_tug_user`
  FOREIGN KEY (`user_id`)
  REFERENCES `toems_users` (`toems_user_id`)
  ON DELETE CASCADE
  ON UPDATE NO ACTION;


"
            ;
        }
    }
}
