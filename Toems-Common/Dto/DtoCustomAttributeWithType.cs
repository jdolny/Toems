using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Enum;

namespace Toems_Common.Dto
{
    public class DtoCustomAttributeWithType
    {
        public int AttributeId { get; set; }
        public string Name { get; set; }
        public string UsageType { get; set; }
        public EnumCustomAttribute.TextMode TextMode { get; set; }
    }
}
