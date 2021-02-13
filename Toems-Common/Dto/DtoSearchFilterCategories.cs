using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoSearchFilterCategories
    {
        public DtoSearchFilterCategories()
        {
            Limit = Int32.MaxValue;
            SearchText = string.Empty;
            CategoryType = "Any Category";
            Categories = new List<string>();
        }

        public string SearchText { get; set; }
        public int Limit { get; set; }
        public string CategoryType { get; set; }
        public List<string> Categories { get; set; }
        public string AssetType { get; set; }
        public bool IncludeOus { get; set; }
    }
}
