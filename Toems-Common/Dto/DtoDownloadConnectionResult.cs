using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoDownloadConnectionResult
    {
        public DtoDownloadConnectionResult()
        {
            Success = false;
            QueueIsFull = false;
        }
        public bool Success { get; set; }
        public bool QueueIsFull { get; set; }
        public string ErrorMessage { get; set; }
    }
}
