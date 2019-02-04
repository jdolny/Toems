using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoPinnedGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int MemberCount { get; set; }
        public string Description { get; set; }
    }
}
