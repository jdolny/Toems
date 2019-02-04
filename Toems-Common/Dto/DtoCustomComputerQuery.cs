using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoCustomComputerQuery
    {
         
        public int Order { get; set; }
        public string AndOr { get; set; }
        public string LeftParenthesis { get; set; }
        public string Table { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string RightParenthesis { get; set; }
        public bool IncludeArchived { get; set; }
        public bool IncludePreProvisioned { get; set; }
        public string GroupBy { get; set; }
    }
}
