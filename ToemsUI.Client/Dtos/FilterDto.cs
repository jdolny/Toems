using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToemsUI.Client.Dtos
{
    public class FilterDto
    {
        public FilterDto()
        {
            Categories = new List<string>();
        }
        public string CategoryFilterType { get; set; }
        public List<string> Categories { get; set; }
        public string SearchText { get; set; }
        public int Limit { get; set; }
    }
}
