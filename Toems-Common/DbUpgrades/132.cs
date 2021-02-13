using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    class _132 : IDbScript
    {
        public string Get()
        {
            return
@"UPDATE `toems_version` SET `expected_app_version` = '1.3.2.0', `database_version` = '1.3.2.0', `expected_toecapi_version` = '1.3.2.0' WHERE (`toems_version_id` = '1');

INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('NewProvisionADCheck', '0');
INSERT INTO `admin_settings` (`admin_setting_name`, `admin_setting_value`) VALUES ('NewProvisonDefaultGroup', '');

INSERT INTO `groups` (`group_id`, `group_name`, `group_description`, `is_ou`, `group_type`, `cluster_id`, `wakeup_schedule_id`, `shutdown_schedule_id`, `prevent_shutdown`, `imaging_priority`, `em_priority`, `image_id`, `image_profile_id`) VALUES ('-1', 'Built-In All Computers', 'Built-In group for all computers.  This group will not display any members.', '0', 'Static', '-1', '-1', '-1', '0', '0', '0', '-1', '-1');
";
        }
    }
}
