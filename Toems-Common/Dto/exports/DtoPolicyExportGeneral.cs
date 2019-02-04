using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto.exports
{
    public class DtoPolicyExportGeneral
    {
        public int PolicyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string Requirements { get; set; }
    }
}
