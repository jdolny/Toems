﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _155 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.5.5.0', `database_version` = '1.5.5.0', `expected_toecapi_version` = '1.5.5.0', `latest_client_version` = '1.5.4.0' WHERE (`toems_version_id` = '1');

"
            ;
        }
    }
}
