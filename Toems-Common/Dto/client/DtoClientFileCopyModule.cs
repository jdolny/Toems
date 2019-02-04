using System.Collections.Generic;

namespace Toems_Common.Dto.client
{
    public class DtoClientFileCopyModule
    {
         public DtoClientFileCopyModule()
        {
            Files = new List<DtoClientFileHash>();
        }
        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public string Destination { get; set; }
        public int Order { get; set; }
        public bool Unzip { get; set; }


        public List<DtoClientFileHash> Files { get; set; }
    }
}
