using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoSearchFilter
    {
        public DtoSearchFilter()
        {
            SearchText = string.Empty;
            Limit = Int32.MaxValue;
            
        }
        public string SearchText { get; set; }
        public int Limit { get; set; }
    }
}
