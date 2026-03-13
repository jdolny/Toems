using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoUserComputerManagement
    {
        public bool ComputerManagementEnforced { get; set; } = false;
        public List<int> AllowedComputerIds { get; set; } = new List<int>();
    }
}
