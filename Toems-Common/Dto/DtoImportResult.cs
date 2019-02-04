using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoImportResult
    {
        public DtoImportResult()
        {
            Success = false;
        }

        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public bool HasExternalFiles { get; set; }
        public bool HasInternalFiles { get; set; }
    }
}
