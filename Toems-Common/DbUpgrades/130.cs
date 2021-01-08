using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _130 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.3.0.0', `database_version` = '1.3.0.0', `expected_toecapi_version` = '1.3.0.0' WHERE (`toems_version_id` = '1');

ALTER TABLE `com_server_cluster_servers` 
ADD COLUMN `is_ra_server` TINYINT NOT NULL DEFAULT 0 AFTER `is_em_server`;

ALTER TABLE `client_com_servers` 
ADD COLUMN `is_ra_server` TINYINT NOT NULL DEFAULT 0 AFTER `imaging_ip`;
ADD COLUMN `ra_url` VARCHAR(255) NULL AFTER `is_ra_server`,
ADD COLUMN `ra_username` VARCHAR(45) NULL AFTER `ra_url`,
ADD COLUMN `ra_password_encrypted` VARCHAR(200) NULL AFTER `ra_username`,
ADD COLUMN `ra_auth_header_encrypted` VARCHAR(255) NULL AFTER `ra_password_encrypted`;
ADD COLUMN `ra_organization` VARCHAR(45) NULL AFTER `ra_auth_header_encrypted`;

ALTER TABLE `policies` 
ADD COLUMN `policy_remote_access` TINYINT NULL DEFAULT 0 AFTER `condition_failed_action`;

ALTER TABLE `computers` 
ADD COLUMN `last_socket_result` LONGTEXT NULL AFTER `imaging_mac`;




";
        }
    }
}
