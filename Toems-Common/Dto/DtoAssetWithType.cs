using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoAssetWithType
    {
        public int AssetId { get; set; }
        public string AssetType { get; set; }
        public string Name { get; set; }
        public bool IsArchived { get; set; }
    }
}
