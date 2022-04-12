using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _147 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.4.7.0', `database_version` = '1.4.7.0', `expected_toecapi_version` = '1.4.7.0', `latest_client_version` = '1.4.4.0' WHERE (`toems_version_id` = '1');

CREATE TABLE `sysprep_answer_files` (
  `sysprep_answer_file_id` INT NOT NULL AUTO_INCREMENT,
  `sysprep_answer_file_name` VARCHAR(45) NOT NULL,
  `sysprep_answer_file_description` TEXT NULL,
  `sysprep_answer_file_contents` TEXT NULL,
  PRIMARY KEY (`sysprep_answer_file_id`));

CREATE TABLE `setupcomplete_files` (
  `setupcomplete_file_id` INT NOT NULL AUTO_INCREMENT,
  `setupcomplete_file_name` VARCHAR(45) NOT NULL,
  `setupcomplete_file_description` TEXT NULL,
  `setupcomplete_file_contents` TEXT NULL,
  PRIMARY KEY (`setupcomplete_file_id`));
"
            ;
        }
    }
}
