using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto.formdata
{
    public class DtoPrepareUpload
    {
        public int taskId { get; set; } 
        public string fileName { get; set; }
        public int profileId { get; set; }
        public int userId { get; set; }
        public string hdNumber { get; set; }
    }
}
