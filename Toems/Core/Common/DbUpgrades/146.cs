using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _146 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.4.6.0', `database_version` = '1.4.6.0', `expected_toecapi_version` = '1.4.6.0', `latest_client_version` = '1.4.4.0' WHERE (`toems_version_id` = '1');

ALTER TABLE `computers` 
ADD COLUMN `pxe_ip_address` VARCHAR(45) NULL DEFAULT NULL AFTER `last_socket_result`,
ADD COLUMN `pxe_netmask` VARCHAR(45) NULL DEFAULT NULL AFTER `pxe_ip_address`,
ADD COLUMN `pxe_gateway` VARCHAR(45) NULL DEFAULT NULL AFTER `pxe_netmask`,
ADD COLUMN `pxe_dns` VARCHAR(45) NULL DEFAULT NULL AFTER `pxe_gateway`;


"
            ;
        }
    }
}
