using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;

namespace Toems_Common.Dto
{
    public class DtoImageSchemaRequest
    {
        public EntityImage image { get; set; }
        public ImageProfileWithImage imageProfile { get; set; }
        public string schemaType { get; set; }
        public string selectedHd { get; set; }
    }
}
