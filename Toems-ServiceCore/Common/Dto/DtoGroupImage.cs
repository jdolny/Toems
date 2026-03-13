using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoGroupImage
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDn { get; set; }
        public int ImagePriority { get;  set; }
        public int EmPriority { get; set; }
        public string ImageName { get; set; }
        public string ProfileName { get; set; }
    }
}
