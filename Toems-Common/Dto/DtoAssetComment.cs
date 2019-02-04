using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoAssetComment
    {
        public string Comment { get; set; }
        public int AssetId { get; set; }
        public string Username { get; set; }
        public DateTime CommentTime { get; set; }
    }
}
