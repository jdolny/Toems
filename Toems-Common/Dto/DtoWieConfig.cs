using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoWieConfig
    {
        public string Timezone { get; set; } = String.Empty;
        public string InputLocale { get; set; } = String.Empty;
        public string Language { get; set; } = String.Empty;
        public string ComServers { get; set; } = String.Empty;
        public string Token { get; set; } = String.Empty;
        public bool RestrictComServers { get; set; }
        public List<int> Drivers { get; set; } = new List<int>();
        public int ImpersonationId { get; set; }
    }
}
