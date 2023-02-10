using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoUserGroupManagement
    {
        public bool GroupManagementEnforced { get; set; } = false;
        public List<int> AllowedGroupIds { get; set; } = new List<int>();
    }
}
