using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoUserImageManagement
    {
        public bool ImageManagementEnforced { get; set; } = false;
        public List<int> AllowedImageIds { get; set; } = new List<int>();
    }
}
