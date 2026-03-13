using System.Collections.Generic;

namespace Toems_Common.Dto
{
    public class DtoWolTask
    {
        public DtoWolTask()
        {
            Macs = new List<string>();
        }

        public string Gateway { get; set; }
        public List<string> Macs { get; set; } 
    }
}
